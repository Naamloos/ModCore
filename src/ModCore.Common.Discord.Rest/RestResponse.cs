namespace ModCore.Common.Discord.Rest
{
    public struct RestResponse<T>
    {
        public T? Value { get; private set; }
        public HttpResponseMessage HttpResponse { get; private set; }
        public bool Success => HttpResponse.IsSuccessStatusCode;
        public string RawBody => HttpResponse.Content.ReadAsStringAsync().Result;

        internal RestResponse(T? value, HttpResponseMessage response)
        {
            this.Value = value;
            this.HttpResponse = response;
        }
    }
}
