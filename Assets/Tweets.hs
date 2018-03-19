{-# LANGUAGE LambdaCase, OverloadedStrings, TemplateHaskell #-}

module Main (main) where

import Control.Lens (_last, rewrite, (^.), (&), (?~), (^?), (.~))
import Control.Monad (when)
import Data.Aeson (ToJSON, defaultOptions, encode, fieldLabelModifier)
import Data.Aeson.TH (deriveJSON)
import qualified Data.ByteString.Char8 as B
import qualified Data.ByteString.Lazy.Char8 as LB
import Data.Char (isAscii)
import qualified Data.Conduit.List as CL
import Data.Foldable (traverse_)
import Data.List (stripPrefix)
import qualified Data.Text as T
import qualified Data.Text.IO as T
import System.Environment (getArgs, getEnv)
import System.IO (hPrint, stderr)
import Web.Twitter.Conduit
import qualified Web.Twitter.Conduit.Parameters as P
import Web.Twitter.Types.Lens

getTokens :: IO OAuth
getTokens = do
    key <- getEnv "OAUTH_CONSUMER_KEY"
    secret <- getEnv "OAUTH_CONSUMER_SECRET"
    pure $ twitterOAuth
        { oauthConsumerKey = B.pack key
        , oauthConsumerSecret = B.pack secret
        }

getCredential :: IO Credential
getCredential = do
    token <- getEnv "OAUTH_ACCESS_TOKEN"
    secret <- getEnv "OAUTH_ACCESS_SECRET"
    pure $ Credential
        [ ("oauth_token", B.pack token)
        , ("oauth_token_secret", B.pack secret)
        ]

getTwInfo :: IO TWInfo
getTwInfo = do
    tokens <- getTokens
    credential <- getCredential
    pure $ setCredential tokens credential def

getTweets :: TWInfo -> Manager -> [(String, Int)] -> IO [Status]
getTweets twInfo mgr = go Nothing
  where
    go _ [] = pure []
    go _ ((_, 0) : xs) = go Nothing xs
    go maxId ((name, n) : xs) = do
        tweets <- call twInfo mgr
            $ userTimeline (ScreenNameParam name) & P.count ?~ 200 & P.maxId .~ maxId
        case tweets ^? _last . statusId of
            Nothing -> pure tweets
            maxId' -> (tweets ++) <$> go maxId' ((name, n - 1) : xs)

tweetFilter :: Status -> Bool
tweetFilter tweet = not (tweet ^. statusTruncated) && T.length (tweet ^. statusText) < 120

statusModifier :: String -> String
statusModifier = filter isAscii
    . reverse . dropWhile (== ' ') . reverse . dropWhile (== ' ')
    . rewrite (fmap ('&' :) . stripPrefix "&amp;")
    . rewrite (fmap (dropWhile (/= ' ')) . stripPrefix "http")

parseArgs :: [String] -> [(String, Int)]
parseArgs (name : n : xs) = (name, read n) : parseArgs xs
parseArgs _ = []

data Tweet = Tweet
    { _name :: String
    , _id :: Integer
    , _status :: String
    }
deriveJSON defaultOptions {fieldLabelModifier = drop 1} ''Tweet

main :: IO ()
main = do
    twInfo <- getTwInfo
    mgr <- newManager tlsManagerSettings
    args <- parseArgs <$> getArgs
    tweets <- filter tweetFilter <$> getTweets twInfo mgr args
    let results = (\t -> Tweet
            { _name = T.unpack $ t ^. statusUser . userScreenName
            , _id = t ^. statusId
            , _status = statusModifier . T.unpack $ t ^. statusText
            }) <$> tweets
    LB.putStrLn $ encode results
