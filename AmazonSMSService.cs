using Amazon;
using Amazon.SimpleNotificationService.Model;
using Amazon.SimpleNotificationService;
using _3ai.solutions.Core.Interfaces;
using _3ai.solutions.Core.Models;

namespace _3ai.solutions.AmazonSMS
{
    public class AmazonSMSService : ISMSService
    {
        private readonly string _accessKey;
        private readonly string _secretAccessKey;
        private readonly RegionEndpoint _region;
        
         public enum SMSTypeCode
        {
            Transactional = 1,
            Promotional = 2
        }

        public AmazonSMSService(AmazonS3Options options)
        {
            _accessKey = options.AccessKey;
            _secretAccessKey = options.SecretAccessKey;
            _region = RegionEndpoint.GetBySystemName(options.Region);
        }

        public async Task<Result> SendSMSAsync(string message, string number, CancellationToken token = default)
        {
            using AmazonSimpleNotificationServiceClient client =   new(_accessKey, _secretAccessKey, _region);
            PublishRequest request = new ()
            {
                Message = message,
                PhoneNumber = number,
            };
            var response = await client.PublishAsync(request, token);
            return ConvertResponse(response);
        }
        
        //SenderId
        public async Task<Result> SendSMSAsync(string message, string number, string senderId, CancellationToken token = default)
        {
            using AmazonSimpleNotificationServiceClient client = new(_accessKey, _secretAccessKey, _region);
            PublishRequest request = new()
            {
                Message = message,
                PhoneNumber = number,
            };
            request.MessageAttributes.Add("AWS.SNS.SMS.SenderID", new MessageAttributeValue { StringValue = senderId, DataType = "String" });
            var response = await client.PublishAsync(request, token);
            return ConvertResponse(response);
        }

        //SenderId and Transaction Type
        public async Task<Result> SendSMSAsync(string message, string number, string senderId, string smsType, CancellationToken token = default)
        {
            using AmazonSimpleNotificationServiceClient client = new(_accessKey, _secretAccessKey, _region);
            PublishRequest request = new()
            {
                Message = message,
                PhoneNumber = number,
            };
            request.MessageAttributes.Add("AWS.SNS.SMS.SenderID", new MessageAttributeValue { StringValue = senderId, DataType = "String" });
            request.MessageAttributes.Add("AWS.SNS.SMS.SMSType", new MessageAttributeValue { StringValue = smsType, DataType = "String" });
            var response = await client.PublishAsync(request, token);

            return ConvertResponse(response);
        }

        private Result ConvertResponse(PublishResponse? response)
        {
            if (response == null)
                return new Result() { Success = false, Message = "No response" };
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                return new Result() { Success = false, Message = response.HttpStatusCode.ToString(), LongMessage = System.Text.Json.JsonSerializer.Serialize(response.ResponseMetadata) };
            return new Result() { Success = true, Message = response.MessageId, LongMessage = System.Text.Json.JsonSerializer.Serialize(response.ResponseMetadata) };
        }
    }
}
