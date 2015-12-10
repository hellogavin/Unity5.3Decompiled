namespace UnityEngine.Networking.Match
{
    using SimpleJson;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.Networking.Types;

    public class NetworkMatch : MonoBehaviour
    {
        private const string kMultiplayerNetworkingIdKey = "CloudNetworkingId";
        private Uri m_BaseUri = new Uri("https://mm.unet.unity3d.com");

        public Coroutine CreateMatch(CreateMatchRequest req, ResponseDelegate<CreateMatchResponse> callback)
        {
            Uri uri = new Uri(this.baseUri, "/json/reply/CreateMatchRequest");
            Debug.Log("MatchMakingClient Create :" + uri);
            WWWForm form = new WWWForm();
            form.AddField("projectId", Application.cloudProjectId);
            form.AddField("sourceId", Utility.GetSourceID().ToString());
            form.AddField("appId", Utility.GetAppID().ToString());
            form.AddField("accessTokenString", 0);
            form.AddField("domain", 0);
            form.AddField("name", req.name);
            form.AddField("size", req.size.ToString());
            form.AddField("advertise", req.advertise.ToString());
            form.AddField("password", req.password);
            form.headers["Accept"] = "application/json";
            WWW client = new WWW(uri.ToString(), form);
            return base.StartCoroutine(this.ProcessMatchResponse<CreateMatchResponse>(client, callback));
        }

        public Coroutine CreateMatch(string matchName, uint matchSize, bool matchAdvertise, string matchPassword, ResponseDelegate<CreateMatchResponse> callback)
        {
            CreateMatchRequest req = new CreateMatchRequest {
                name = matchName,
                size = matchSize,
                advertise = matchAdvertise,
                password = matchPassword
            };
            return this.CreateMatch(req, callback);
        }

        public Coroutine DestroyMatch(DestroyMatchRequest req, ResponseDelegate<BasicResponse> callback)
        {
            Uri uri = new Uri(this.baseUri, "/json/reply/DestroyMatchRequest");
            Debug.Log("MatchMakingClient Destroy :" + uri.ToString());
            WWWForm form = new WWWForm();
            form.AddField("projectId", Application.cloudProjectId);
            form.AddField("sourceId", Utility.GetSourceID().ToString());
            form.AddField("appId", Utility.GetAppID().ToString());
            form.AddField("accessTokenString", Utility.GetAccessTokenForNetwork(req.networkId).GetByteString());
            form.AddField("domain", 0);
            form.AddField("networkId", req.networkId.ToString());
            form.headers["Accept"] = "application/json";
            WWW client = new WWW(uri.ToString(), form);
            return base.StartCoroutine(this.ProcessMatchResponse<BasicResponse>(client, callback));
        }

        public Coroutine DestroyMatch(NetworkID netId, ResponseDelegate<BasicResponse> callback)
        {
            DestroyMatchRequest req = new DestroyMatchRequest {
                networkId = netId
            };
            return this.DestroyMatch(req, callback);
        }

        public Coroutine DropConnection(DropConnectionRequest req, ResponseDelegate<BasicResponse> callback)
        {
            Uri uri = new Uri(this.baseUri, "/json/reply/DropConnectionRequest");
            Debug.Log("MatchMakingClient DropConnection :" + uri);
            WWWForm form = new WWWForm();
            form.AddField("projectId", Application.cloudProjectId);
            form.AddField("sourceId", Utility.GetSourceID().ToString());
            form.AddField("appId", Utility.GetAppID().ToString());
            form.AddField("accessTokenString", Utility.GetAccessTokenForNetwork(req.networkId).GetByteString());
            form.AddField("domain", 0);
            form.AddField("networkId", req.networkId.ToString());
            form.AddField("nodeId", req.nodeId.ToString());
            form.headers["Accept"] = "application/json";
            WWW client = new WWW(uri.ToString(), form);
            return base.StartCoroutine(this.ProcessMatchResponse<BasicResponse>(client, callback));
        }

        public Coroutine DropConnection(NetworkID netId, NodeID dropNodeId, ResponseDelegate<BasicResponse> callback)
        {
            DropConnectionRequest req = new DropConnectionRequest {
                networkId = netId,
                nodeId = dropNodeId
            };
            return this.DropConnection(req, callback);
        }

        public Coroutine JoinMatch(JoinMatchRequest req, ResponseDelegate<JoinMatchResponse> callback)
        {
            Uri uri = new Uri(this.baseUri, "/json/reply/JoinMatchRequest");
            Debug.Log("MatchMakingClient Join :" + uri);
            WWWForm form = new WWWForm();
            form.AddField("projectId", Application.cloudProjectId);
            form.AddField("sourceId", Utility.GetSourceID().ToString());
            form.AddField("appId", Utility.GetAppID().ToString());
            form.AddField("accessTokenString", 0);
            form.AddField("domain", 0);
            form.AddField("networkId", req.networkId.ToString());
            form.AddField("password", req.password);
            form.headers["Accept"] = "application/json";
            WWW client = new WWW(uri.ToString(), form);
            return base.StartCoroutine(this.ProcessMatchResponse<JoinMatchResponse>(client, callback));
        }

        public Coroutine JoinMatch(NetworkID netId, string matchPassword, ResponseDelegate<JoinMatchResponse> callback)
        {
            JoinMatchRequest req = new JoinMatchRequest {
                networkId = netId,
                password = matchPassword
            };
            return this.JoinMatch(req, callback);
        }

        public Coroutine ListMatches(ListMatchRequest req, ResponseDelegate<ListMatchResponse> callback)
        {
            Uri uri = new Uri(this.baseUri, "/json/reply/ListMatchRequest");
            Debug.Log("MatchMakingClient ListMatches :" + uri);
            WWWForm form = new WWWForm();
            form.AddField("projectId", Application.cloudProjectId);
            form.AddField("sourceId", Utility.GetSourceID().ToString());
            form.AddField("appId", Utility.GetAppID().ToString());
            form.AddField("includePasswordMatches", req.includePasswordMatches.ToString());
            form.AddField("accessTokenString", 0);
            form.AddField("domain", 0);
            form.AddField("pageSize", req.pageSize);
            form.AddField("pageNum", req.pageNum);
            form.AddField("nameFilter", req.nameFilter);
            form.headers["Accept"] = "application/json";
            WWW client = new WWW(uri.ToString(), form);
            return base.StartCoroutine(this.ProcessMatchResponse<ListMatchResponse>(client, callback));
        }

        public Coroutine ListMatches(int startPageNumber, int resultPageSize, string matchNameFilter, ResponseDelegate<ListMatchResponse> callback)
        {
            ListMatchRequest req = new ListMatchRequest {
                pageNum = startPageNumber,
                pageSize = resultPageSize,
                nameFilter = matchNameFilter
            };
            return this.ListMatches(req, callback);
        }

        [DebuggerHidden]
        private IEnumerator ProcessMatchResponse<JSONRESPONSE>(WWW client, ResponseDelegate<JSONRESPONSE> callback) where JSONRESPONSE: Response, new()
        {
            return new <ProcessMatchResponse>c__Iterator0<JSONRESPONSE> { client = client, callback = callback, <$>client = client, <$>callback = callback };
        }

        public void SetProgramAppID(AppID programAppID)
        {
            Utility.SetAppID(programAppID);
        }

        public Uri baseUri
        {
            get
            {
                return this.m_BaseUri;
            }
            set
            {
                this.m_BaseUri = value;
            }
        }

        [CompilerGenerated]
        private sealed class <ProcessMatchResponse>c__Iterator0<JSONRESPONSE> : IDisposable, IEnumerator, IEnumerator<object> where JSONRESPONSE: Response, new()
        {
            internal object $current;
            internal int $PC;
            internal NetworkMatch.ResponseDelegate<JSONRESPONSE> <$>callback;
            internal WWW <$>client;
            internal IDictionary<string, object> <dictJsonObj>__2;
            internal FormatException <exception>__3;
            internal JSONRESPONSE <jsonInterface>__0;
            internal object <o>__1;
            internal NetworkMatch.ResponseDelegate<JSONRESPONSE> callback;
            internal WWW client;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = this.client;
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<jsonInterface>__0 = null;
                        if (!string.IsNullOrEmpty(this.client.error))
                        {
                            Debug.LogError("Request error: " + this.client.error);
                            Debug.LogError("Raw response: " + this.client.text);
                            break;
                        }
                        if (SimpleJson.SimpleJson.TryDeserializeObject(this.client.text, out this.<o>__1))
                        {
                            this.<dictJsonObj>__2 = this.<o>__1 as IDictionary<string, object>;
                            if (this.<dictJsonObj>__2 != null)
                            {
                                try
                                {
                                    this.<jsonInterface>__0 = Activator.CreateInstance<JSONRESPONSE>();
                                    this.<jsonInterface>__0.Parse(this.<o>__1);
                                }
                                catch (FormatException exception)
                                {
                                    this.<exception>__3 = exception;
                                    Debug.Log(this.<exception>__3);
                                }
                            }
                        }
                        if (this.<jsonInterface>__0 == null)
                        {
                            Debug.LogError("Could not parse: " + this.client.text);
                        }
                        else
                        {
                            Debug.Log("JSON Response: " + this.<jsonInterface>__0.ToString());
                        }
                        break;

                    default:
                        goto Label_018B;
                }
                if (this.<jsonInterface>__0 == null)
                {
                    this.<jsonInterface>__0 = Activator.CreateInstance<JSONRESPONSE>();
                }
                this.callback(this.<jsonInterface>__0);
                this.$PC = -1;
            Label_018B:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        public delegate void ResponseDelegate<T>(T response);
    }
}

