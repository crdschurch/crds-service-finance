﻿//using System.Net;
//using Moq;
//using Newtonsoft.Json;
//using RestSharp;

//public static IRestClient MockRestSharpClient<T>(HttpStatusCode httpStatusCode, string json) where T : new()
//{
//    //var data = JsonConvert.DeserializeObject<T>(json);
//    //var response = new Mock<IRestResponse<T>>();
//    //response.Setup(_ => _.StatusCode).Returns(httpStatusCode);
//    //response.Setup(_ => _.Data).Returns(data);

//    //var mockIRestClient = new Mock<IRestClient>();
//    //mockIRestClient.Setup(x => x.Execute<T>(It.IsAny<IRestRequest>())).Returns(response.Object);
//    //return mockIRestClient.Object;
//}