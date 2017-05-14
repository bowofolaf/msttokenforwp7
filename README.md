# Microsoft Translator Token for Windows Phone 7

The Microsoft Translator API has been updated and Bing APIs are no longer supported. The following link provides more information about this:
http://msdn.microsoft.com/en-us/library/hh454950

This library simply performs the async Http calls to the Microsoft Translator Token Service to obtain a token that lasts for 10minutes. A handler has to be wired to update the token object in your calling code, and it is advised to put a timer on expiry property of this object to ensure a new token is obtained before it expires.

You need two credentials, a client_secret and a client_id, information about both are contained in the link above.

The documentation of this project will be updated to reflect the use and several changes will be made to add error handling delegates as well.
