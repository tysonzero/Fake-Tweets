{-# LANGUAGE LambdaCase, OverloadedStrings #-}

module Main (main) where

import Control.Lens (_last, rewrite, (^.), (&), (?~), (^?), (.~))
import Control.Monad (when)
import Data.Aeson (encode)
import qualified Data.ByteString.Char8 as B
import qualified Data.ByteString.Lazy.Char8 as LB
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

getTweets :: TWInfo -> Manager -> String -> Int -> IO [Status]
getTweets twInfo mgr name = go Nothing
  where
    go _ 0 = pure []
    go maxId n = do
        tweets <- call twInfo mgr
            $ userTimeline (ScreenNameParam name) & P.count ?~ 200 & P.maxId .~ maxId
        case tweets ^? _last . statusId of
            Nothing -> pure tweets
            maxId' -> (tweets ++) <$> go maxId' (n - 1)

main :: IO ()
main = do
    twInfo <- getTwInfo
    mgr <- newManager tlsManagerSettings
    [name, nStr] <- getArgs
    let n = read nStr
    tweets <- getTweets twInfo mgr name n
    let statuses = T.unpack . (^. statusText) <$> filter (not . (^. statusTruncated)) tweets
    let statuses2 = rewrite (fmap (dropWhile (/= ' ')) . stripPrefix "http") <$> statuses
    LB.putStrLn . encode $ (,) name <$> statuses2
