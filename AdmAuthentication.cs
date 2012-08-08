using System;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace WPhoneMSTAccessToken
{
    [DataContract]
    public class AdmAccessToken
    {
        [DataMember]
        public string access_token { get; set; }
        [DataMember]
        public string token_type { get; set; }
        [DataMember]
        public string expires_in { get; set; }
        [DataMember]
        public string scope { get; set; }
    }

    public class AdmAuthentication
    {
        public delegate void ReturnToken(AdmAccessToken token);
        public event ReturnToken returnToken;
        public static readonly string DatamarketAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
        private string clientId;
        private string clientSecret;
        private string requestString;
        //AdmAccessToken _token;
        //byte[] bytes;

        public AdmAuthentication(string clientId, string clientSecret, ReturnToken returnToken)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.returnToken = returnToken;
            //If clientid or client secret has special characters, encode before sending request
            this.requestString = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", HttpUtility.UrlEncode(clientId), HttpUtility.UrlEncode(clientSecret));
        }

        public void GetToken()
        {
            Start();
        }

        void Start()
        {
            try
            {
                //Create HttpWebRequest object
                HttpWebRequest myRequest1 = (HttpWebRequest)WebRequest.Create(DatamarketAccessUri);
                myRequest1.Method = "POST";
                myRequest1.ContentType = "application/x-www-form-urlencoded";
                

                //Create instance of requestState and assign web request to it
                RequestState state = new RequestState();
                state.request = myRequest1;

                //Get Request Stream
                IAsyncResult result = state.request.BeginGetRequestStream(new AsyncCallback(ReadRequestStream), state);
                //ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, new WaitOrTimerCallback(ScanTimeoutCallback), state, 30 * 1000 , false);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        void ReadRequestStream(IAsyncResult result)
        {
            try
            {
                RequestState state = (RequestState)result.AsyncState;
                HttpWebRequest myRequest2 = state.request;
                Stream streamRequest = myRequest2.EndGetRequestStream(result);

                byte[] bytes = UTF8Encoding.UTF8.GetBytes(requestString);
                streamRequest.Write(bytes,0,requestString.Length);
                streamRequest.Close();
                state.streamRequest = streamRequest;
                myRequest2.BeginGetResponse(new AsyncCallback(ReadResponse), state);



            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        private static void ScanTimeoutCallback(
          object state, bool timedOut)
        {
            if (timedOut)
            {
                RequestState reqState = (RequestState)state;
                if (reqState != null)
                    reqState.request.Abort();
            }
        }

        void ReadResponse(IAsyncResult result)
        {
            try
            {
                RequestState state = (RequestState)result.AsyncState;
                HttpWebRequest myRequest3 = state.request;
                HttpWebResponse response = (HttpWebResponse)myRequest3.EndGetResponse(result);

                Stream responseStream = response.GetResponseStream();
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AdmAccessToken));
                //Get deserialized object from JSON stream
                AdmAccessToken token = (AdmAccessToken)serializer.ReadObject(responseStream);
                returnToken(token);

            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
    }

    internal class RequestState
    {
        // This class stores the State of the request.
        const int BUFFER_SIZE = 1024;
        public StringBuilder requestData;
        public byte[] BufferRead;
        public HttpWebRequest request;
        public HttpWebResponse response;
        public Stream streamRequest;
        public Stream streamResponse;

        public RequestState()
        {
            BufferRead = new byte[BUFFER_SIZE];
            requestData = new StringBuilder("");
            request = null;
            streamRequest = null;
            streamResponse = null;
        }
    }
}

