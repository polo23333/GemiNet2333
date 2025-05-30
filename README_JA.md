# GemiNet
Gemini Developer API client for .NET and Unity

[![NuGet](https://img.shields.io/nuget/v/GemiNet.svg)](https://www.nuget.org/packages/GemiNet)
[![Releases](https://img.shields.io/github/release/nuskey8/GemiNet.svg)](https://github.com/nuskey8/GemiNet/releases)
[![GitHub license](https://img.shields.io/github/license/nuskey8/GemiNet.svg)](./LICENSE)

[English](./README.md) | 日本語

## 概要

GemiNetはNET/Unity向けのGemini Developer APIクライアントライブラリです。GemiNetは公式のTypeScript SDK([js-genai](https://github.com/googleapis/js-genai))のAPIに基づいて設計されており、他のライブラリ([Google_GenerativeAI](https://github.com/gunpal5/Google_GenerativeAI), [GeminiSharp](https://github.com/dprakash2101/GeminiSharp), etc.)と比較して扱いやすいAPIを提供します。また、Microsoft.Extensions.AIの抽象化層に対応した拡張パッケージを提供することで、アプリケーションとの統合をよりスムーズに行えるようになっています。

## インストール

### NuGet packages

GemiNetを利用するには.NET Standard2.1以上が必要です。パッケージはNuGetから入手できます。

### .NET CLI

```ps1
dotnet add package GemiNet
```

### Package Manager

```ps1
Install-Package GemiNet
```

### Unity

NugetForUnityを用いることでUnityでGemiNetを利用可能です。詳細は[Unity](#unity-1)の項目を参照してください。

## クイックスタート

`GoogleGenAI`を用いてGemini APIを呼び出すことが可能です。

```cs
using GemiNet;

using var ai = new GoogleGenAI
{
    ApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY"),
};

var response = await ai.Models.GenerateContentAsync(new()
{
    Model = Models.Gemini2_0Flash,    // models/gemini-2.0-flash
    Contents = "Hello, Gemini!"
});

Console.WriteLine(response.GetText());
```

また、ストリーミング生成にも対応しています。

```cs
var request = new GenerateContentRequest
{
    Model = Models.Gemini2_0Flash,
    Contents = "Hello, Gemini!"
};

await foreach (var response in GenerateContentStreamAsync(request))
{
    Console.WriteLine(response.GetText());
}
```

## 機能一覧

GemiNetの`GoogleGenAI`はTypeScript SDKと同様のモジュールで分割されています。

* `ai.Models`ではコンテンツの生成やベクトル生成、利用可能なモデルの取得などを行えます。
* `ai.Caches`では特定の入力のキャッシュを作成することができます。
* `ai.Files`ではファイルのアップロードや削除などを行えます。
* `ai.Live`ではLive API(WebSocketを用いた双方向通信)を利用できます。詳細は[Live API](#live-api)の項目を参照してください。

## Live API

GemiNetは[Live API](https://ai.google.dev/api/live)に対応しています。メッセージは`ReceiveAsync()`を用いることで`await foreach`で読み取ることが可能です。

```cs
using GemiNet;

using var ai = new GoogleGenAI();

await using var session = await ai.Live.ConnectAsync(new()
{
    Model = Models.Gemini2_0FlashLive,
    Config = new()
    {
        ResponseModalities = [Modality.Text]
    }
});

_ = Task.Run(async () =>
{
    await session.SendRealtimeInputAsync(new()
    {
        Text = "Hello, Gemini!"
    });
});

await foreach (var message in session.ReceiveAsync())
{
    Console.WriteLine(message.ServerContent?.ModelTurn?.Parts[0].Text);
}
```

## Microsoft.Extensions.AI

GemiNetをMicrosoft.Extensions.AIで利用するには`GemiNet.Extensions.AI`パッケージを追加します。

### インストール

#### .NET CLI

```ps1
dotnet add package GemiNet.Extensions.AI
```

#### Package Manager

```ps1
Install-Package GemiNet.Extensions.AI
```

### 使い方

`AsChatClient()`と`AsEmbeddingGenerator()`を用いて`GoogleGenAI`をMicrosoft.Extensions.AIのインターフェースに変換できます。

```cs
using GemiNet;
using GemiNet.Extensions.AI;

using var ai = new GoogleGenAI();

var client = ai.AsChatClient(Models.Gemini2_0Flash);
var response = await client.GetResponseAsync([new(ChatRole.User, "What is AI?")]);

var embeddingGenerator = ai.AsEmbeddingGenerator(Models.Gemini2_0Flash);
var embedding = await embeddingGenerator.GenerateEmbeddingAsync("Hello, Gemini!");
```

## 制限事項

* このライブラリは現在Vertex AIに対応していません。Vertex AIを利用したい場合は[Google.Cloud.AIPlatform.V1](https://www.nuget.org/packages/Google.Cloud.AIPlatform.V1/)を代わりに使用してください。

## ライセンス

このライブラリは[MITライセンス](./LICENSE)の下に公開されています。