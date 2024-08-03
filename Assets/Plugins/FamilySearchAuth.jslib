mergeInto(LibraryManager.library,
{
	StartAuthentication: function (utf8String)
	{
		var authRequest = UTF8ToString(utf8String);
		var authorizationRequest = authRequest;
		StartOAuth(authorizationRequest);
	}
});