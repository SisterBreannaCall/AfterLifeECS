using JWT;
using Unity.Entities;
using JWT.Serializers;
using Newtonsoft.Json;
using IdentityTokenResource;

public partial class FSDecodeJwtSystem : SystemBase
{
    protected override void OnCreate()
    {
        Enabled = false;
    }

    protected override void OnStartRunning()
    {
        RefRW<FSComponent> fsComponent = SystemAPI.GetSingletonRW<FSComponent>();
        RefRW<IdentityComponent> identityComponent = SystemAPI.GetSingletonRW<IdentityComponent>();

        IJsonSerializer serializer = new JsonNetSerializer();
        IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
        IJwtDecoder decoder = new JwtDecoder(serializer, urlEncoder);

        string jwtString = decoder.Decode(fsComponent.ValueRO.idToken.ToString());

        IdentityJson identityJson = JsonConvert.DeserializeObject<IdentityJson>(jwtString);

        identityComponent.ValueRW.sub = identityJson.sub;
        identityComponent.ValueRW.country = identityJson.country;
        identityComponent.ValueRW.gender = identityJson.gender;
        identityComponent.ValueRW.iss = identityJson.iss;
        identityComponent.ValueRW.language = identityJson.language;
        identityComponent.ValueRW.sessionId = identityJson.sessionId;
        identityComponent.ValueRW.givenName = identityJson.given_name;
        identityComponent.ValueRW.locale = identityJson.locale;
        identityComponent.ValueRW.aud = identityJson.aud;
        identityComponent.ValueRW.affiliateAccount = identityJson.qualifies_for_affiliate_account;
        identityComponent.ValueRW.exp = identityJson.exp.ToString();
        identityComponent.ValueRW.iat = identityJson.iat.ToString();
        identityComponent.ValueRW.familyName = identityJson.family_name;
        identityComponent.ValueRW.email = identityJson.email;

        World.GetExistingSystemManaged<FSCurrentUserSystem>().Enabled = true;

        Enabled = false;
    }

    protected override void OnUpdate()
    {
        
    }
}
