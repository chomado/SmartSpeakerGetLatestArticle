# スマートスピーカースキル『最新のブログ記事』

＊ ストアには提出していないので、ストアで検索しても無いです

スマートスピーカーで [ちょまどブログ](https://chomado.com/author/chomado/)の最新記事を取ってきてくれる

![スマートスピーカースキル開発](image/howItWorks/03.JPG)

1. ユーザー「最新の記事を教えて」
1. Azure Functions 発火
1. 私のブログ [chomado.com](https://chomado.com/author/chomado/) の RSS を読み込んで、最新記事のタイトルを取得する
1. スマートスピーカー（LINE Clova/ Google Home/ Amazon Alexa) が、そのタイトルを読み上げてくれる
1. LINE にメッセージも送ってくれる (LINE Clova の時のみ)

## アーキテクチャ

![スマートスピーカースキル開発](image/demo01.jpg)
