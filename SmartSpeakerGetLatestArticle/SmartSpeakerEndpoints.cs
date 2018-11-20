
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CEK.CSharp;
using CEK.CSharp.Models;
using TechSummit2018.ServerlessSmartSpeaker.Services;
using Google.Protobuf;
using Google.Cloud.Dialogflow.V2;
using Google.Protobuf.WellKnownTypes;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Line.Messaging;
using System.Collections.Generic;
using SmartSpeakerGetLatestArticle.Models;

namespace TechSummit2018.ServerlessSmartSpeaker
{
    public static class SmartSpeakerEndpoints
    {
        private static string IntroductionMessage { get; } = "こんにちは、LINEデベロッパー・デイのデモアプリです。最新記事を教えてと聞いてください。";
        private static string HelloMessage { get; } = "こんにちは、ちょまどさん！";
        private static string ErrorMessage { get; } = "すみません、わかりませんでした！";

        [FunctionName("Line")]
        public static async Task<IActionResult> Line([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, ILogger log)
        {
            var client = new ClovaClient();
            var cekRequest = await client.GetRequest(req.Headers["SignatureCEK"], req.Body);
            var cekResponse = new CEKResponse();
            switch (cekRequest.Request.Type)
            {
                case RequestType.LaunchRequest:
                    cekResponse.AddText(IntroductionMessage);
                    cekResponse.ShouldEndSession = false;
                    break;
                case RequestType.IntentRequest:
                    {
                        var r = await HandleIntentAsync(cekRequest.Request.Intent.Name);
                        cekResponse.AddText(r.ResponseMessage);

                        if (r.Blog != null)
                        {
                            // 最新記事が見つかったので LINE にプッシュ通知する
                            string secret = "";
                            var messagingClient = new LineMessagingClient(secret);
                            await messagingClient.PushMessageAsync(
                                to: cekRequest.Session.User.UserId,
                                messages: new List<ISendMessage>
                                {
                                    new TextMessage($"ちょまどさんの最新記事はこちら！"),
                                    new TextMessage($@"タイトル『{r.Blog.Title}』
{r.Blog.Url}"),
                                });
                        }
                    }
                    break;
            }

            return new OkObjectResult(cekResponse);
        }


        [FunctionName("GoogleHome")]
        public static async Task<IActionResult> GoogleHome([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, ILogger log)
        {
            var parser = new JsonParser(JsonParser.Settings.Default.WithIgnoreUnknownFields(true));
            var webhookRequest = parser.Parse<WebhookRequest>(await req.ReadAsStringAsync());
            var webhookResponse = new WebhookResponse();
            log.LogInformation(webhookRequest.QueryResult.Intent.DisplayName);
            switch (webhookRequest.QueryResult.Intent.DisplayName)
            {
                case "Default Welcome Intent":
                    webhookResponse.FulfillmentText = IntroductionMessage;
                    break;
                default:
                    {
                        var r = await HandleIntentAsync(webhookRequest.QueryResult.Intent.DisplayName);
                        webhookResponse.FulfillmentText = r.ResponseMessage;
                    }
                    break;
            }

            return new ProtcolBufJsonResult(webhookResponse, JsonFormatter.Default);
        }

        [FunctionName("Alexa")]
        public static async Task<IActionResult> Alexa([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, ILogger log)
        {
            var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(await new StreamReader(req.Body).ReadToEndAsync());
            var skillResponse = new SkillResponse
            {
                Version = "1.0",
                Response = new ResponseBody(),
            };
            switch (skillRequest.Request)
            {
                case LaunchRequest lr:
                    skillResponse.Response.OutputSpeech = new PlainTextOutputSpeech
                    {
                        Text = IntroductionMessage,
                    };
                    break;
                case IntentRequest ir:
                    {
                        var r = await HandleIntentAsync(ir.Intent.Name);
                        skillResponse.Response.OutputSpeech = new PlainTextOutputSpeech
                        {
                            Text = r.ResponseMessage,
                        };
                    }
                    break;
                default:
                    skillResponse.Response.OutputSpeech = new PlainTextOutputSpeech
                    {
                        Text = "すいません。わかりません。",
                    };
                    break;
            }

            return new OkObjectResult(skillResponse);
        }

        private static async Task<HandleIntentResult> HandleIntentAsync(string intent)
        {
            switch (intent)
            {
                case "HelloIntent":
                    return new HandleIntentResult { ResponseMessage = HelloMessage };
                case "AskLatestBlogTitleIntent":
                    {
                        var chomadoBlogService = new ChomadoBlogService();
                        var blog = await chomadoBlogService.GetLatestBlogAsync();

                        if (blog != null)
                        {
                            return new HandleIntentResult
                            {
                                ResponseMessage = $"ちょまどさんのブログの最新記事は {blog.Title} です。",
                                Blog = blog,
                            };
                        }
                        else
                        {
                            return new HandleIntentResult
                            {
                                ResponseMessage = "ちょまどさんのブログの最新記事は、わかりませんでした。",
                            };
                        }
                    }
                default:
                    return new HandleIntentResult
                    {
                        ResponseMessage = ErrorMessage,
                    };
            }
            
        }
    }
}
