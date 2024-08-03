using System;
using System.Net;
using System.Text;
using UnityEngine;
using Unity.Entities;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

public partial class FSLoginSystem : SystemBase
{
#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void StartAuthentication(string authRequest);
#else

#endif

    protected override void OnCreate()
    {
        Enabled = false;
    }

    protected override void OnUpdate()
    {
       
    }

    protected override void OnStartRunning()
    {
        RefRW<FSComponent> fsComponent = SystemAPI.GetSingletonRW<FSComponent>();

        int randomState = UnityEngine.Random.Range(2000000, 3000000);
        string outState = randomState.ToString();
        fsComponent.ValueRW.outState = outState;

        string codeVerifier = GenerateRandom(32);
        string codeChallenge = EncodeNoPadding(GenerateSha256(codeVerifier));
        fsComponent.ValueRW.codeVerifier = codeVerifier;

#if UNITY_WEBGL

        string redirectUri = "https://jonathangibsonstudios.com/theafterlifeai/callback.html";

        string authReuqest = string.Format("{0}?client_id={1}&redirect_uri={2}&response_type=code&state={3}&code_challenge={4}&code_challenge_method=S256&scope=openid%20profile%20email%20qualifies_for_affiliate_account%20country",
            fsComponent.ValueRO.authProduction,
            fsComponent.ValueRO.clientID,
            redirectUri,
            outState,
            codeChallenge);

        StartAuthentication(authReuqest);

#elif UNITY_EDITOR || UNITY_STANDALONE_WIN
        HttpListener httpListener;

        string redirectUri = "http://127.0.0.1:5000";
        string redirectUriListener = "http://127.0.0.1:5000/";

        httpListener = new HttpListener();
        httpListener.Prefixes.Add(redirectUriListener);

        string authRequest = string.Format("{0}?client_id={1}&redirect_uri={2}&response_type=code&state={3}&code_challenge={4}&code_challenge_method=S256&scope=openid%20profile%20email%20qualifies_for_affiliate_account%20country",
            fsComponent.ValueRO.authProduction,
            fsComponent.ValueRO.clientID,
            redirectUri,
            fsComponent.ValueRO.outState,
            codeChallenge);

        httpListener.Start();

        Application.OpenURL(authRequest);

        HttpListenerContext context = httpListener.GetContext();
        HttpListenerResponse response = context.Response;

        string responseString = "<HTML><HEAD><SCRIPT>window.close();</SCRIPT></HEAD><BODY></BODY></HTML>";
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        System.IO.Stream output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();

        httpListener.Stop();

        string inState = context.Request.QueryString.Get("state");

        if (inState == fsComponent.ValueRO.outState)
        {
            fsComponent.ValueRW.authCode = context.Request.QueryString.Get("code");
            World.GetExistingSystemManaged<FSAccessSystem>().Enabled = true;
        }

        Enabled = false;
#endif
    }

    public void GetAuthResultsWeb(string result)
    {
        RefRW<FSComponent> fsComponent = SystemAPI.GetSingletonRW<FSComponent>();

        string[] response = result.Split('?');

        response = response[1].Split("&");

        string[] authResponse = response[0].Split("=");
        string[] stateResponse = response[1].Split("=");

        if (stateResponse[1] == fsComponent.ValueRO.outState)
        {
            fsComponent.ValueRW.authCode = authResponse[1];
            World.GetExistingSystemManaged<FSAccessSystem>().Enabled = true;
        }

        Enabled = false;
    }

    private static string GenerateRandom(uint length)
    {
        byte[] bytes = new byte[length];
        RandomNumberGenerator.Create().GetBytes(bytes);
        return EncodeNoPadding(bytes);
    }

    private static string EncodeNoPadding(byte[] buffer)
    {
        string toEncode = Convert.ToBase64String(buffer);

        toEncode = toEncode.Replace("+", "-");
        toEncode = toEncode.Replace("/", "_");
        toEncode = toEncode.Replace("=", "");

        return toEncode;
    }

    private static byte[] GenerateSha256(string inputString)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(inputString);
        SHA256 sha256 = SHA256.Create();
        return sha256.ComputeHash(bytes);
    }
}
