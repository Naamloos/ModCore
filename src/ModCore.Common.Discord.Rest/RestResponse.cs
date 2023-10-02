namespace ModCore.Common.Discord.Rest
{
    public struct RestResponse<T>
    {
        public T? Value { get; private set; }
        public HttpResponseMessage HttpResponse { get; private set; }
        public bool Success => HttpResponse.IsSuccessStatusCode;

        internal RestResponse(T? value, HttpResponseMessage response)
        {
            this.Value = value;
            this.HttpResponse = response;
        }
    }
}
