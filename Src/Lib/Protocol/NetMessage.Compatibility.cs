namespace SkillBridge.Message
{
    public sealed partial class NetMessage
    {
        public NetMessageRequest? Request
        {
            get => Requests.Count == 0 ? null : Requests[0];
            set
            {
                Requests.Clear();
                if (value != null)
                {
                    Requests.Add(value);
                }
            }
        }

        public NetMessageResponse? Response
        {
            get => Responses.Count == 0 ? null : Responses[0];
            set
            {
                Responses.Clear();
                if (value != null)
                {
                    Responses.Add(value);
                }
            }
        }

        public bool HasResponse(NetMessageResponse.PayloadOneofCase payloadCase)
        {
            foreach (NetMessageResponse response in Responses)
            {
                if (response.PayloadCase == payloadCase)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
