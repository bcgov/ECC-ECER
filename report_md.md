# ZAP Scanning Report


## Summary of Alerts

| Risk Level | Number of Alerts |
| --- | --- |
| High | 0 |
| Medium | 5 |
| Low | 4 |
| Informational | 12 |




## Alerts

| Name | Risk Level | Number of Instances |
| --- | --- | --- |
| Content Security Policy (CSP) Header Not Set | Medium | 1 |
| Hidden File Found | Medium | 1 |
| Missing Anti-clickjacking Header | Medium | 1 |
| Proxy Disclosure | Medium | 11 |
| Relative Path Confusion | Medium | 7 |
| Cookie with SameSite Attribute None | Low | 1 |
| Permissions Policy Header Not Set | Low | 2 |
| Timestamp Disclosure - Unix | Low | 1 |
| X-Content-Type-Options Header Missing | Low | 6 |
| Base64 Disclosure | Informational | 2 |
| Cookie Slack Detector | Informational | 11 |
| Information Disclosure - Suspicious Comments | Informational | 4 |
| Modern Web Application | Informational | 1 |
| Non-Storable Content | Informational | 1 |
| Re-examine Cache-control Directives | Informational | 1 |
| Sec-Fetch-Dest Header is Missing | Informational | 2 |
| Sec-Fetch-Mode Header is Missing | Informational | 2 |
| Sec-Fetch-Site Header is Missing | Informational | 2 |
| Sec-Fetch-User Header is Missing | Informational | 2 |
| Session Management Response Identified | Informational | 2 |
| Storable and Cacheable Content | Informational | 7 |




## Alert Detail



### [ Content Security Policy (CSP) Header Not Set ](https://www.zaproxy.org/docs/alerts/10038/)



##### Medium (High)

### Description

Content Security Policy (CSP) is an added layer of security that helps to detect and mitigate certain types of attacks, including Cross Site Scripting (XSS) and data injection attacks. These attacks are used for everything from data theft to site defacement or distribution of malware. CSP provides a set of standard HTTP headers that allow website owners to declare approved sources of content that browsers should be allowed to load on that page — covered types are JavaScript, CSS, HTML frames, fonts, images and embeddable objects such as Java applets, ActiveX, audio and video files.

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: ``

Instances: 1

### Solution

Ensure that your web server, application server, load balancer, etc. is configured to set the Content-Security-Policy header.

### Reference


* [ https://developer.mozilla.org/en-US/docs/Web/Security/CSP/Introducing_Content_Security_Policy ](https://developer.mozilla.org/en-US/docs/Web/Security/CSP/Introducing_Content_Security_Policy)
* [ https://cheatsheetseries.owasp.org/cheatsheets/Content_Security_Policy_Cheat_Sheet.html ](https://cheatsheetseries.owasp.org/cheatsheets/Content_Security_Policy_Cheat_Sheet.html)
* [ https://www.w3.org/TR/CSP/ ](https://www.w3.org/TR/CSP/)
* [ https://w3c.github.io/webappsec-csp/ ](https://w3c.github.io/webappsec-csp/)
* [ https://web.dev/articles/csp ](https://web.dev/articles/csp)
* [ https://caniuse.com/#feat=contentsecuritypolicy ](https://caniuse.com/#feat=contentsecuritypolicy)
* [ https://content-security-policy.com/ ](https://content-security-policy.com/)


#### CWE Id: [ 693 ](https://cwe.mitre.org/data/definitions/693.html)


#### WASC Id: 15

#### Source ID: 3

### [ Hidden File Found ](https://www.zaproxy.org/docs/alerts/40035/)



##### Medium (Low)

### Description

A sensitive file was identified as accessible or available. This may leak administrative, configuration, or credential information which can be leveraged by a malicious individual to further attack the system or conduct social engineering efforts.

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/BitKeeper
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `HTTP/1.1 200 OK`
  * Other Info: ``

Instances: 1

### Solution

Consider whether or not the component is actually required in production, if it isn't then disable it. If it is then ensure access to it requires appropriate authentication and authorization, or limit exposure to internal systems or specific source IPs, etc.

### Reference


* [ https://blog.hboeck.de/archives/892-Introducing-Snallygaster-a-Tool-to-Scan-for-Secrets-on-Web-Servers.html ](https://blog.hboeck.de/archives/892-Introducing-Snallygaster-a-Tool-to-Scan-for-Secrets-on-Web-Servers.html)


#### CWE Id: [ 538 ](https://cwe.mitre.org/data/definitions/538.html)


#### WASC Id: 13

#### Source ID: 1

### [ Missing Anti-clickjacking Header ](https://www.zaproxy.org/docs/alerts/10020/)



##### Medium (Medium)

### Description

The response does not include either Content-Security-Policy with 'frame-ancestors' directive or X-Frame-Options to protect against 'ClickJacking' attacks.

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/
  * Method: `GET`
  * Parameter: `x-frame-options`
  * Attack: ``
  * Evidence: ``
  * Other Info: ``

Instances: 1

### Solution

Modern Web browsers support the Content-Security-Policy and X-Frame-Options HTTP headers. Ensure one of them is set on all web pages returned by your site/app.
If you expect the page to be framed only by pages on your server (e.g. it's part of a FRAMESET) then you'll want to use SAMEORIGIN, otherwise if you never expect the page to be framed, you should use DENY. Alternatively consider implementing Content Security Policy's "frame-ancestors" directive.

### Reference


* [ https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options ](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options)


#### CWE Id: [ 1021 ](https://cwe.mitre.org/data/definitions/1021.html)


#### WASC Id: 15

#### Source ID: 3

### [ Proxy Disclosure ](https://www.zaproxy.org/docs/alerts/40025/)



##### Medium (Medium)

### Description

1 proxy server(s) were detected or fingerprinted. This information helps a potential attacker to determine 
 - A list of targets for an attack against the application.
 - Potential vulnerabilities on the proxy servers that service the application.
 - The presence or absence of any proxy-based components that might cause attacks against the application to be detected, prevented, or mitigated. 

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca
  * Method: `GET`
  * Parameter: ``
  * Attack: `TRACE, OPTIONS methods with 'Max-Forwards' header. TRACK method.`
  * Evidence: ``
  * Other Info: `Using the TRACE, OPTIONS, and TRACK methods, the following proxy servers have been identified between ZAP and the application/web server: 
- Unknown
The following web/application server has been identified: 
- Kestrel
`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/
  * Method: `GET`
  * Parameter: ``
  * Attack: `TRACE, OPTIONS methods with 'Max-Forwards' header. TRACK method.`
  * Evidence: ``
  * Other Info: `Using the TRACE, OPTIONS, and TRACK methods, the following proxy servers have been identified between ZAP and the application/web server: 
- Unknown
The following web/application server has been identified: 
- Kestrel
`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets
  * Method: `GET`
  * Parameter: ``
  * Attack: `TRACE, OPTIONS methods with 'Max-Forwards' header. TRACK method.`
  * Evidence: ``
  * Other Info: `Using the TRACE, OPTIONS, and TRACK methods, the following proxy servers have been identified between ZAP and the application/web server: 
- Unknown
The following web/application server has been identified: 
- Kestrel
`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-288d27d7.css
  * Method: `GET`
  * Parameter: ``
  * Attack: `TRACE, OPTIONS methods with 'Max-Forwards' header. TRACK method.`
  * Evidence: ``
  * Other Info: `Using the TRACE, OPTIONS, and TRACK methods, the following proxy servers have been identified between ZAP and the application/web server: 
- Unknown
The following web/application server has been identified: 
- Kestrel
`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-eb629945.js
  * Method: `GET`
  * Parameter: ``
  * Attack: `TRACE, OPTIONS methods with 'Max-Forwards' header. TRACK method.`
  * Evidence: ``
  * Other Info: `Using the TRACE, OPTIONS, and TRACK methods, the following proxy servers have been identified between ZAP and the application/web server: 
- Unknown
The following web/application server has been identified: 
- Kestrel
`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/bcid-apple-touch-icon.png
  * Method: `GET`
  * Parameter: ``
  * Attack: `TRACE, OPTIONS methods with 'Max-Forwards' header. TRACK method.`
  * Evidence: ``
  * Other Info: `Using the TRACE, OPTIONS, and TRACK methods, the following proxy servers have been identified between ZAP and the application/web server: 
- Unknown
The following web/application server has been identified: 
- Kestrel
`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/favicon.ico
  * Method: `GET`
  * Parameter: ``
  * Attack: `TRACE, OPTIONS methods with 'Max-Forwards' header. TRACK method.`
  * Evidence: ``
  * Other Info: `Using the TRACE, OPTIONS, and TRACK methods, the following proxy servers have been identified between ZAP and the application/web server: 
- Unknown
The following web/application server has been identified: 
- Kestrel
`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/robots.txt
  * Method: `GET`
  * Parameter: ``
  * Attack: `TRACE, OPTIONS methods with 'Max-Forwards' header. TRACK method.`
  * Evidence: ``
  * Other Info: `Using the TRACE, OPTIONS, and TRACK methods, the following proxy servers have been identified between ZAP and the application/web server: 
- Unknown
The following web/application server has been identified: 
- Kestrel
`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/sitemap.xml
  * Method: `GET`
  * Parameter: ``
  * Attack: `TRACE, OPTIONS methods with 'Max-Forwards' header. TRACK method.`
  * Evidence: ``
  * Other Info: `Using the TRACE, OPTIONS, and TRACK methods, the following proxy servers have been identified between ZAP and the application/web server: 
- Unknown
The following web/application server has been identified: 
- Kestrel
`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/styles
  * Method: `GET`
  * Parameter: ``
  * Attack: `TRACE, OPTIONS methods with 'Max-Forwards' header. TRACK method.`
  * Evidence: ``
  * Other Info: `Using the TRACE, OPTIONS, and TRACK methods, the following proxy servers have been identified between ZAP and the application/web server: 
- Unknown
The following web/application server has been identified: 
- Kestrel
`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/styles/fonts.css
  * Method: `GET`
  * Parameter: ``
  * Attack: `TRACE, OPTIONS methods with 'Max-Forwards' header. TRACK method.`
  * Evidence: ``
  * Other Info: `Using the TRACE, OPTIONS, and TRACK methods, the following proxy servers have been identified between ZAP and the application/web server: 
- Unknown
The following web/application server has been identified: 
- Kestrel
`

Instances: 11

### Solution

Disable the 'TRACE' method on the proxy servers, as well as the origin web/application server.
Disable the 'OPTIONS' method on the proxy servers, as well as the origin web/application server, if it is not required for other purposes, such as 'CORS' (Cross Origin Resource Sharing).
Configure the web and application servers with custom error pages, to prevent 'fingerprintable' product-specific error pages being leaked to the user in the event of HTTP errors, such as 'TRACK' requests for non-existent pages.
Configure all proxies, application servers, and web servers to prevent disclosure of the technology and version information in the 'Server' and 'X-Powered-By' HTTP response headers.


### Reference


* [ https://tools.ietf.org/html/rfc7231#section-5.1.2 ](https://tools.ietf.org/html/rfc7231#section-5.1.2)


#### CWE Id: [ 200 ](https://cwe.mitre.org/data/definitions/200.html)


#### WASC Id: 45

#### Source ID: 1

### [ Relative Path Confusion ](https://www.zaproxy.org/docs/alerts/10051/)



##### Medium (Medium)

### Description

The web server is configured to serve responses to ambiguous URLs in a manner that is likely to lead to confusion about the correct "relative path" for the URL. Resources (CSS, images, etc.) are also specified in the page response using relative, rather than absolute URLs. In an attack, if the web browser parses the "cross-content" response in a permissive manner, or can be tricked into permissively parsing the "cross-content" response, using techniques such as framing, then the web browser may be fooled into interpreting HTML as CSS (or other content types), leading to an XSS vulnerability.

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-288d27d7.css
  * Method: `GET`
  * Parameter: ``
  * Attack: `https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-288d27d7.css/6w2j1/41ht5`
  * Evidence: `<link rel="apple-touch-icon" href="bcid-apple-touch-icon.png">`
  * Other Info: `No <base> tag was specified in the HTML <head> tag to define the location for relative URLs.
A Content Type of "text/html" was specified. If the web browser is employing strict parsing rules, this will prevent cross-content attacks from succeeding. Quirks Mode in the web browser would disable strict parsing.  
No X-Frame-Options header was specified, so the page can be framed, and this can be used to enable Quirks Mode, allowing the specified Content Type to be bypassed.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-eb629945.js
  * Method: `GET`
  * Parameter: ``
  * Attack: `https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-eb629945.js/6w2j1/41ht5`
  * Evidence: `<link rel="apple-touch-icon" href="bcid-apple-touch-icon.png">`
  * Other Info: `No <base> tag was specified in the HTML <head> tag to define the location for relative URLs.
A Content Type of "text/html" was specified. If the web browser is employing strict parsing rules, this will prevent cross-content attacks from succeeding. Quirks Mode in the web browser would disable strict parsing.  
No X-Frame-Options header was specified, so the page can be framed, and this can be used to enable Quirks Mode, allowing the specified Content Type to be bypassed.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/bcid-apple-touch-icon.png
  * Method: `GET`
  * Parameter: ``
  * Attack: `https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/bcid-apple-touch-icon.png/6w2j1/41ht5`
  * Evidence: `<link rel="apple-touch-icon" href="bcid-apple-touch-icon.png">`
  * Other Info: `No <base> tag was specified in the HTML <head> tag to define the location for relative URLs.
A Content Type of "text/html" was specified. If the web browser is employing strict parsing rules, this will prevent cross-content attacks from succeeding. Quirks Mode in the web browser would disable strict parsing.  
No X-Frame-Options header was specified, so the page can be framed, and this can be used to enable Quirks Mode, allowing the specified Content Type to be bypassed.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/favicon.ico
  * Method: `GET`
  * Parameter: ``
  * Attack: `https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/favicon.ico/6w2j1/41ht5`
  * Evidence: `<link rel="apple-touch-icon" href="bcid-apple-touch-icon.png">`
  * Other Info: `No <base> tag was specified in the HTML <head> tag to define the location for relative URLs.
A Content Type of "text/html" was specified. If the web browser is employing strict parsing rules, this will prevent cross-content attacks from succeeding. Quirks Mode in the web browser would disable strict parsing.  
No X-Frame-Options header was specified, so the page can be framed, and this can be used to enable Quirks Mode, allowing the specified Content Type to be bypassed.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/robots.txt
  * Method: `GET`
  * Parameter: ``
  * Attack: `https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/robots.txt/6w2j1/41ht5`
  * Evidence: `<link rel="apple-touch-icon" href="bcid-apple-touch-icon.png">`
  * Other Info: `No <base> tag was specified in the HTML <head> tag to define the location for relative URLs.
A Content Type of "text/html" was specified. If the web browser is employing strict parsing rules, this will prevent cross-content attacks from succeeding. Quirks Mode in the web browser would disable strict parsing.  
No X-Frame-Options header was specified, so the page can be framed, and this can be used to enable Quirks Mode, allowing the specified Content Type to be bypassed.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/sitemap.xml
  * Method: `GET`
  * Parameter: ``
  * Attack: `https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/sitemap.xml/6w2j1/41ht5`
  * Evidence: `<link rel="apple-touch-icon" href="bcid-apple-touch-icon.png">`
  * Other Info: `No <base> tag was specified in the HTML <head> tag to define the location for relative URLs.
A Content Type of "text/html" was specified. If the web browser is employing strict parsing rules, this will prevent cross-content attacks from succeeding. Quirks Mode in the web browser would disable strict parsing.  
No X-Frame-Options header was specified, so the page can be framed, and this can be used to enable Quirks Mode, allowing the specified Content Type to be bypassed.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/styles/fonts.css
  * Method: `GET`
  * Parameter: ``
  * Attack: `https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/styles/fonts.css/6w2j1/41ht5`
  * Evidence: `<link rel="apple-touch-icon" href="bcid-apple-touch-icon.png">`
  * Other Info: `No <base> tag was specified in the HTML <head> tag to define the location for relative URLs.
A Content Type of "text/html" was specified. If the web browser is employing strict parsing rules, this will prevent cross-content attacks from succeeding. Quirks Mode in the web browser would disable strict parsing.  
No X-Frame-Options header was specified, so the page can be framed, and this can be used to enable Quirks Mode, allowing the specified Content Type to be bypassed.`

Instances: 7

### Solution

Web servers and frameworks should be updated to be configured to not serve responses to ambiguous URLs in such a way that the relative path of such URLs could be mis-interpreted by components on either the client side, or server side.
Within the application, the correct use of the "<base>" HTML tag in the HTTP response will unambiguously specify the base URL for all relative URLs in the document.
Use the "Content-Type" HTTP response header to make it harder for the attacker to force the web browser to mis-interpret the content type of the response.
Use the "X-Content-Type-Options: nosniff" HTTP response header to prevent the web browser from "sniffing" the content type of the response.
Use a modern DOCTYPE such as "<!doctype html>" to prevent the page from being rendered in the web browser using "Quirks Mode", since this results in the content type being ignored by the web browser.
Specify the "X-Frame-Options" HTTP response header to prevent Quirks Mode from being enabled in the web browser using framing attacks. 

### Reference


* [ http://www.thespanner.co.uk/2014/03/21/rpo/ ](http://www.thespanner.co.uk/2014/03/21/rpo/)
* [ https://hsivonen.fi/doctype/ ](https://hsivonen.fi/doctype/)
* [ http://www.w3schools.com/tags/tag_base.asp ](http://www.w3schools.com/tags/tag_base.asp)


#### CWE Id: [ 20 ](https://cwe.mitre.org/data/definitions/20.html)


#### WASC Id: 20

#### Source ID: 1

### [ Cookie with SameSite Attribute None ](https://www.zaproxy.org/docs/alerts/10054/)



##### Low (Medium)

### Description

A cookie has been set with its SameSite attribute set to "none", which means that the cookie can be sent as a result of a 'cross-site' request. The SameSite attribute is an effective counter measure to cross-site request forgery, cross-site script inclusion, and timing attacks.

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/
  * Method: `GET`
  * Parameter: `fae215f6a98faf034d267e6fb0d680d6`
  * Attack: ``
  * Evidence: `set-cookie: fae215f6a98faf034d267e6fb0d680d6`
  * Other Info: ``

Instances: 1

### Solution

Ensure that the SameSite attribute is set to either 'lax' or ideally 'strict' for all cookies.

### Reference


* [ https://tools.ietf.org/html/draft-ietf-httpbis-cookie-same-site ](https://tools.ietf.org/html/draft-ietf-httpbis-cookie-same-site)


#### CWE Id: [ 1275 ](https://cwe.mitre.org/data/definitions/1275.html)


#### WASC Id: 13

#### Source ID: 3

### [ Permissions Policy Header Not Set ](https://www.zaproxy.org/docs/alerts/10063/)



##### Low (Medium)

### Description

Permissions Policy Header is an added layer of security that helps to restrict from unauthorized access or usage of browser/client features by web resources. This policy ensures the user privacy by limiting or specifying the features of the browsers can be used by the web resources. Permissions Policy provides a set of standard HTTP headers that allow website owners to limit which features of browsers can be used by the page such as camera, microphone, location, full screen etc.

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: ``
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-eb629945.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: ``

Instances: 2

### Solution

Ensure that your web server, application server, load balancer, etc. is configured to set the Permissions-Policy header.

### Reference


* [ https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Permissions-Policy ](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Permissions-Policy)
* [ https://developer.chrome.com/blog/feature-policy/ ](https://developer.chrome.com/blog/feature-policy/)
* [ https://scotthelme.co.uk/a-new-security-header-feature-policy/ ](https://scotthelme.co.uk/a-new-security-header-feature-policy/)
* [ https://w3c.github.io/webappsec-feature-policy/ ](https://w3c.github.io/webappsec-feature-policy/)
* [ https://www.smashingmagazine.com/2018/12/feature-policy/ ](https://www.smashingmagazine.com/2018/12/feature-policy/)


#### CWE Id: [ 693 ](https://cwe.mitre.org/data/definitions/693.html)


#### WASC Id: 15

#### Source ID: 3

### [ Timestamp Disclosure - Unix ](https://www.zaproxy.org/docs/alerts/10096/)



##### Low (Low)

### Description

A timestamp was disclosed by the application/web server - Unix

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-288d27d7.css
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1428571429`
  * Other Info: `1428571429, which evaluates to: 2015-04-09 09:23:49`

Instances: 1

### Solution

Manually confirm that the timestamp data is not sensitive, and that the data cannot be aggregated to disclose exploitable patterns.

### Reference


* [ http://projects.webappsec.org/w/page/13246936/Information%20Leakage ](http://projects.webappsec.org/w/page/13246936/Information%20Leakage)


#### CWE Id: [ 200 ](https://cwe.mitre.org/data/definitions/200.html)


#### WASC Id: 13

#### Source ID: 3

### [ X-Content-Type-Options Header Missing ](https://www.zaproxy.org/docs/alerts/10021/)



##### Low (Medium)

### Description

The Anti-MIME-Sniffing header X-Content-Type-Options was not set to 'nosniff'. This allows older versions of Internet Explorer and Chrome to perform MIME-sniffing on the response body, potentially causing the response body to be interpreted and displayed as a content type other than the declared content type. Current (early 2014) and legacy versions of Firefox will use the declared content type (if one is set), rather than performing MIME-sniffing.

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/
  * Method: `GET`
  * Parameter: `x-content-type-options`
  * Attack: ``
  * Evidence: ``
  * Other Info: `This issue still applies to error type pages (401, 403, 500, etc.) as those pages are often still affected by injection issues, in which case there is still concern for browsers sniffing pages away from their actual content type.
At "High" threshold this scan rule will not alert on client or server error responses.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-288d27d7.css
  * Method: `GET`
  * Parameter: `x-content-type-options`
  * Attack: ``
  * Evidence: ``
  * Other Info: `This issue still applies to error type pages (401, 403, 500, etc.) as those pages are often still affected by injection issues, in which case there is still concern for browsers sniffing pages away from their actual content type.
At "High" threshold this scan rule will not alert on client or server error responses.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-eb629945.js
  * Method: `GET`
  * Parameter: `x-content-type-options`
  * Attack: ``
  * Evidence: ``
  * Other Info: `This issue still applies to error type pages (401, 403, 500, etc.) as those pages are often still affected by injection issues, in which case there is still concern for browsers sniffing pages away from their actual content type.
At "High" threshold this scan rule will not alert on client or server error responses.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/bcid-apple-touch-icon.png
  * Method: `GET`
  * Parameter: `x-content-type-options`
  * Attack: ``
  * Evidence: ``
  * Other Info: `This issue still applies to error type pages (401, 403, 500, etc.) as those pages are often still affected by injection issues, in which case there is still concern for browsers sniffing pages away from their actual content type.
At "High" threshold this scan rule will not alert on client or server error responses.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/favicon.ico
  * Method: `GET`
  * Parameter: `x-content-type-options`
  * Attack: ``
  * Evidence: ``
  * Other Info: `This issue still applies to error type pages (401, 403, 500, etc.) as those pages are often still affected by injection issues, in which case there is still concern for browsers sniffing pages away from their actual content type.
At "High" threshold this scan rule will not alert on client or server error responses.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/styles/fonts.css
  * Method: `GET`
  * Parameter: `x-content-type-options`
  * Attack: ``
  * Evidence: ``
  * Other Info: `This issue still applies to error type pages (401, 403, 500, etc.) as those pages are often still affected by injection issues, in which case there is still concern for browsers sniffing pages away from their actual content type.
At "High" threshold this scan rule will not alert on client or server error responses.`

Instances: 6

### Solution

Ensure that the application/web server sets the Content-Type header appropriately, and that it sets the X-Content-Type-Options header to 'nosniff' for all web pages.
If possible, ensure that the end user uses a standards-compliant and modern web browser that does not perform MIME-sniffing at all, or that can be directed by the web application/web server to not perform MIME-sniffing.

### Reference


* [ https://learn.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/compatibility/gg622941(v=vs.85) ](https://learn.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/compatibility/gg622941(v=vs.85))
* [ https://owasp.org/www-community/Security_Headers ](https://owasp.org/www-community/Security_Headers)


#### CWE Id: [ 693 ](https://cwe.mitre.org/data/definitions/693.html)


#### WASC Id: 15

#### Source ID: 3

### [ Base64 Disclosure ](https://www.zaproxy.org/docs/alerts/10094/)



##### Informational (Medium)

### Description

Base64 encoded data was disclosed by the application/web server. Note: in the interests of performance not all base64 strings in the response were analyzed individually, the entire response should be looked at by the analyst/security team/developer(s).

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-288d27d7.css
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `iVBORw0KGgoAAAANSUhEUgAAAAgAAAAICAYAAADED76LAAAAAXNSR0IArs4c6QAAACRJREFUKFNjPHTo0H8GJGBnZ8eIzGekgwJk+0BsdCtRHEQbBQBbbh0dIGKknQAAAABJRU5ErkJggg==`
  * Other Info: `�PNG

   IHDR         ���   sRGB ���   $IDAT(Sc<t��$`ggǈ�g��d�@lt+QD [n b��    IEND�B`�`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-eb629945.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `assets/Certifications-56aaf768`
  * Other Info: `j����z�b~'�*'��zi���`

Instances: 2

### Solution

Manually confirm that the Base64 data does not leak sensitive information, and that the data cannot be aggregated/used to exploit other vulnerabilities.

### Reference


* [ https://projects.webappsec.org/w/page/13246936/Information%20Leakage ](https://projects.webappsec.org/w/page/13246936/Information%20Leakage)


#### CWE Id: [ 200 ](https://cwe.mitre.org/data/definitions/200.html)


#### WASC Id: 13

#### Source ID: 3

### [ Cookie Slack Detector ](https://www.zaproxy.org/docs/alerts/90027/)



##### Informational (Low)

### Description

Repeated GET requests: drop a different cookie each time, followed by normal request with all cookies to stabilize session, compare responses against original baseline GET. This can reveal areas where cookie based authentication/attributes are not actually enforced.

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `Cookies that don't have expected effects can reveal flaws in application logic. In the worst case, this can reveal where authentication via cookie token(s) is not actually enforced.
These cookies affected the response: 
These cookies did NOT affect the response: fae215f6a98faf034d267e6fb0d680d6
`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `Cookies that don't have expected effects can reveal flaws in application logic. In the worst case, this can reveal where authentication via cookie token(s) is not actually enforced.
These cookies affected the response: 
These cookies did NOT affect the response: fae215f6a98faf034d267e6fb0d680d6
`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `Cookies that don't have expected effects can reveal flaws in application logic. In the worst case, this can reveal where authentication via cookie token(s) is not actually enforced.
These cookies affected the response: 
These cookies did NOT affect the response: fae215f6a98faf034d267e6fb0d680d6
`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-288d27d7.css
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `Cookies that don't have expected effects can reveal flaws in application logic. In the worst case, this can reveal where authentication via cookie token(s) is not actually enforced.
These cookies affected the response: 
These cookies did NOT affect the response: fae215f6a98faf034d267e6fb0d680d6
`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-eb629945.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `Cookies that don't have expected effects can reveal flaws in application logic. In the worst case, this can reveal where authentication via cookie token(s) is not actually enforced.
These cookies affected the response: 
These cookies did NOT affect the response: fae215f6a98faf034d267e6fb0d680d6
`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/bcid-apple-touch-icon.png
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `Cookies that don't have expected effects can reveal flaws in application logic. In the worst case, this can reveal where authentication via cookie token(s) is not actually enforced.
These cookies affected the response: 
These cookies did NOT affect the response: fae215f6a98faf034d267e6fb0d680d6
`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/favicon.ico
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `Cookies that don't have expected effects can reveal flaws in application logic. In the worst case, this can reveal where authentication via cookie token(s) is not actually enforced.
These cookies affected the response: 
These cookies did NOT affect the response: fae215f6a98faf034d267e6fb0d680d6
`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/robots.txt
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `Cookies that don't have expected effects can reveal flaws in application logic. In the worst case, this can reveal where authentication via cookie token(s) is not actually enforced.
These cookies affected the response: 
These cookies did NOT affect the response: fae215f6a98faf034d267e6fb0d680d6
`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/sitemap.xml
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `Cookies that don't have expected effects can reveal flaws in application logic. In the worst case, this can reveal where authentication via cookie token(s) is not actually enforced.
These cookies affected the response: 
These cookies did NOT affect the response: fae215f6a98faf034d267e6fb0d680d6
`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/styles
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `Cookies that don't have expected effects can reveal flaws in application logic. In the worst case, this can reveal where authentication via cookie token(s) is not actually enforced.
These cookies affected the response: 
These cookies did NOT affect the response: fae215f6a98faf034d267e6fb0d680d6
`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/styles/fonts.css
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `Cookies that don't have expected effects can reveal flaws in application logic. In the worst case, this can reveal where authentication via cookie token(s) is not actually enforced.
These cookies affected the response: 
These cookies did NOT affect the response: fae215f6a98faf034d267e6fb0d680d6
`

Instances: 11

### Solution



### Reference


* [ https://cwe.mitre.org/data/definitions/205.html ](https://cwe.mitre.org/data/definitions/205.html)


#### CWE Id: [ 200 ](https://cwe.mitre.org/data/definitions/200.html)


#### WASC Id: 45

#### Source ID: 1

### [ Information Disclosure - Suspicious Comments ](https://www.zaproxy.org/docs/alerts/10027/)



##### Informational (Low)

### Description

The response appears to contain suspicious comments which may help an attacker. Note: Matches made within script blocks or files are against the entire content not only comments.

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-eb629945.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `Db`
  * Other Info: `The following pattern was used: \bDB\b and was detected in the element starting with: "`)}get[Symbol.toStringTag](){return"AxiosHeaders"}static from(t){return t instanceof this?t:new this(t)}static concat(t,...n){co", see evidence field for the suspicious comment/snippet.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-eb629945.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `from`
  * Other Info: `The following pattern was used: \bFROM\b and was detected in the element starting with: "Expected #hex, #hexa, rgb(), rgba(), hsl(), hsla(), object or number`)}function Zn(e){const{h:t,s:n,v:r,a}=e,i=o=>{const s=(o+t/", see evidence field for the suspicious comment/snippet.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-eb629945.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `query`
  * Other Info: `The following pattern was used: \bQUERY\b and was detected 4 times, the first in the element starting with: "`)}function gv(e){const t=e.dark?2:1,n=e.dark?1:2,r=[];for(const[a,i]of Object.entries(e.colors)){const l=vn(i);r.push(`--v-them", see evidence field for the suspicious comment/snippet.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-eb629945.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `select`
  * Other Info: `The following pattern was used: \bSELECT\b and was detected 2 times, the first in the element starting with: "(function(){const t=document.createElement("link").relList;if(t&&t.supports&&t.supports("modulepreload"))return;for(const a of d", see evidence field for the suspicious comment/snippet.`

Instances: 4

### Solution

Remove all comments that return information that may help an attacker and fix any underlying problems they refer to.

### Reference



#### CWE Id: [ 200 ](https://cwe.mitre.org/data/definitions/200.html)


#### WASC Id: 13

#### Source ID: 3

### [ Modern Web Application ](https://www.zaproxy.org/docs/alerts/10109/)



##### Informational (Medium)

### Description

The application appears to be a modern web application. If you need to explore it automatically then the Ajax Spider may well be more effective than the standard one.

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `<script type="module" crossorigin src="/assets/index-eb629945.js"></script>`
  * Other Info: `No links have been found while there are scripts, which is an indication that this is a modern web application.`

Instances: 1

### Solution

This is an informational alert and so no changes are required.

### Reference




#### Source ID: 3

### [ Non-Storable Content ](https://www.zaproxy.org/docs/alerts/10049/)



##### Informational (Medium)

### Description

The response contents are not storable by caching components such as proxy servers. If the response does not contain sensitive, personal or user-specific information, it may benefit from being stored and cached, to improve performance.

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `private`
  * Other Info: ``

Instances: 1

### Solution

The content may be marked as storable by ensuring that the following conditions are satisfied:
The request method must be understood by the cache and defined as being cacheable ("GET", "HEAD", and "POST" are currently defined as cacheable)
The response status code must be understood by the cache (one of the 1XX, 2XX, 3XX, 4XX, or 5XX response classes are generally understood)
The "no-store" cache directive must not appear in the request or response header fields
For caching by "shared" caches such as "proxy" caches, the "private" response directive must not appear in the response
For caching by "shared" caches such as "proxy" caches, the "Authorization" header field must not appear in the request, unless the response explicitly allows it (using one of the "must-revalidate", "public", or "s-maxage" Cache-Control response directives)
In addition to the conditions above, at least one of the following conditions must also be satisfied by the response:
It must contain an "Expires" header field
It must contain a "max-age" response directive
For "shared" caches such as "proxy" caches, it must contain a "s-maxage" response directive
It must contain a "Cache Control Extension" that allows it to be cached
It must have a status code that is defined as cacheable by default (200, 203, 204, 206, 300, 301, 404, 405, 410, 414, 501).   

### Reference


* [ https://datatracker.ietf.org/doc/html/rfc7234 ](https://datatracker.ietf.org/doc/html/rfc7234)
* [ https://datatracker.ietf.org/doc/html/rfc7231 ](https://datatracker.ietf.org/doc/html/rfc7231)
* [ https://www.w3.org/Protocols/rfc2616/rfc2616-sec13.html ](https://www.w3.org/Protocols/rfc2616/rfc2616-sec13.html)


#### CWE Id: [ 524 ](https://cwe.mitre.org/data/definitions/524.html)


#### WASC Id: 13

#### Source ID: 3

### [ Re-examine Cache-control Directives ](https://www.zaproxy.org/docs/alerts/10015/)



##### Informational (Low)

### Description

The cache-control header has not been set properly or is missing, allowing the browser and proxies to cache content. For static assets like css, js, or image files this might be intended, however, the resources should be reviewed to ensure that no sensitive content will be cached.

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/
  * Method: `GET`
  * Parameter: `cache-control`
  * Attack: ``
  * Evidence: `private`
  * Other Info: ``

Instances: 1

### Solution

For secure content, ensure the cache-control HTTP header is set with "no-cache, no-store, must-revalidate". If an asset should be cached consider setting the directives "public, max-age, immutable".

### Reference


* [ https://cheatsheetseries.owasp.org/cheatsheets/Session_Management_Cheat_Sheet.html#web-content-caching ](https://cheatsheetseries.owasp.org/cheatsheets/Session_Management_Cheat_Sheet.html#web-content-caching)
* [ https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Cache-Control ](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Cache-Control)
* [ https://grayduck.mn/2021/09/13/cache-control-recommendations/ ](https://grayduck.mn/2021/09/13/cache-control-recommendations/)


#### CWE Id: [ 525 ](https://cwe.mitre.org/data/definitions/525.html)


#### WASC Id: 13

#### Source ID: 3

### [ Sec-Fetch-Dest Header is Missing ](https://www.zaproxy.org/docs/alerts/90005/)



##### Informational (High)

### Description

Specifies how and where the data would be used. For instance, if the value is audio, then the requested resource must be audio data and not any other type of resource.

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/
  * Method: `GET`
  * Parameter: `Sec-Fetch-Dest`
  * Attack: ``
  * Evidence: ``
  * Other Info: ``
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/sitemap.xml
  * Method: `GET`
  * Parameter: `Sec-Fetch-Dest`
  * Attack: ``
  * Evidence: ``
  * Other Info: ``

Instances: 2

### Solution

Ensure that Sec-Fetch-Dest header is included in request headers.

### Reference


* [ https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Sec-Fetch-Dest ](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Sec-Fetch-Dest)


#### CWE Id: [ 352 ](https://cwe.mitre.org/data/definitions/352.html)


#### WASC Id: 9

#### Source ID: 3

### [ Sec-Fetch-Mode Header is Missing ](https://www.zaproxy.org/docs/alerts/90005/)



##### Informational (High)

### Description

Allows to differentiate between requests for navigating between HTML pages and requests for loading resources like images, audio etc.

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/
  * Method: `GET`
  * Parameter: `Sec-Fetch-Mode`
  * Attack: ``
  * Evidence: ``
  * Other Info: ``
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/sitemap.xml
  * Method: `GET`
  * Parameter: `Sec-Fetch-Mode`
  * Attack: ``
  * Evidence: ``
  * Other Info: ``

Instances: 2

### Solution

Ensure that Sec-Fetch-Mode header is included in request headers.

### Reference


* [ https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Sec-Fetch-Mode ](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Sec-Fetch-Mode)


#### CWE Id: [ 352 ](https://cwe.mitre.org/data/definitions/352.html)


#### WASC Id: 9

#### Source ID: 3

### [ Sec-Fetch-Site Header is Missing ](https://www.zaproxy.org/docs/alerts/90005/)



##### Informational (High)

### Description

Specifies the relationship between request initiator's origin and target's origin.

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/
  * Method: `GET`
  * Parameter: `Sec-Fetch-Site`
  * Attack: ``
  * Evidence: ``
  * Other Info: ``
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/sitemap.xml
  * Method: `GET`
  * Parameter: `Sec-Fetch-Site`
  * Attack: ``
  * Evidence: ``
  * Other Info: ``

Instances: 2

### Solution

Ensure that Sec-Fetch-Site header is included in request headers.

### Reference


* [ https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Sec-Fetch-Site ](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Sec-Fetch-Site)


#### CWE Id: [ 352 ](https://cwe.mitre.org/data/definitions/352.html)


#### WASC Id: 9

#### Source ID: 3

### [ Sec-Fetch-User Header is Missing ](https://www.zaproxy.org/docs/alerts/90005/)



##### Informational (High)

### Description

Specifies if a navigation request was initiated by a user.

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/
  * Method: `GET`
  * Parameter: `Sec-Fetch-User`
  * Attack: ``
  * Evidence: ``
  * Other Info: ``
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/sitemap.xml
  * Method: `GET`
  * Parameter: `Sec-Fetch-User`
  * Attack: ``
  * Evidence: ``
  * Other Info: ``

Instances: 2

### Solution

Ensure that Sec-Fetch-User header is included in user initiated requests.

### Reference


* [ https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Sec-Fetch-User ](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Sec-Fetch-User)


#### CWE Id: [ 352 ](https://cwe.mitre.org/data/definitions/352.html)


#### WASC Id: 9

#### Source ID: 3

### [ Session Management Response Identified ](https://www.zaproxy.org/docs/alerts/10112/)



##### Informational (Medium)

### Description

The given response has been identified as containing a session management token. The 'Other Info' field contains a set of header tokens that can be used in the Header Based Session Management Method. If the request is in a context which has a Session Management Method set to "Auto-Detect" then this rule will change the session management to use the tokens identified.

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/
  * Method: `GET`
  * Parameter: `fae215f6a98faf034d267e6fb0d680d6`
  * Attack: ``
  * Evidence: `00432b656e3d0b408b503985a4fbee58`
  * Other Info: `
cookie:fae215f6a98faf034d267e6fb0d680d6`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/
  * Method: `GET`
  * Parameter: `fae215f6a98faf034d267e6fb0d680d6`
  * Attack: ``
  * Evidence: `00432b656e3d0b408b503985a4fbee58`
  * Other Info: `
cookie:fae215f6a98faf034d267e6fb0d680d6`

Instances: 2

### Solution

This is an informational alert rather than a vulnerability and so there is nothing to fix.

### Reference


* [ https://www.zaproxy.org/docs/desktop/addons/authentication-helper/session-mgmt-id ](https://www.zaproxy.org/docs/desktop/addons/authentication-helper/session-mgmt-id)



#### Source ID: 3

### [ Storable and Cacheable Content ](https://www.zaproxy.org/docs/alerts/10049/)



##### Informational (Medium)

### Description

The response contents are storable by caching components such as proxy servers, and may be retrieved directly from the cache, rather than from the origin server by the caching servers, in response to similar requests from other users.  If the response data is sensitive, personal or user-specific, this may result in sensitive information being leaked. In some cases, this may even result in a user gaining complete control of the session of another user, depending on the configuration of the caching components in use in their environment. This is primarily an issue where "shared" caching servers such as "proxy" caches are configured on the local network. This configuration is typically found in corporate or educational environments, for instance.

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-288d27d7.css
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `In the absence of an explicitly specified caching lifetime directive in the response, a liberal lifetime heuristic of 1 year was assumed. This is permitted by rfc7234.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-eb629945.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `In the absence of an explicitly specified caching lifetime directive in the response, a liberal lifetime heuristic of 1 year was assumed. This is permitted by rfc7234.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/bcid-apple-touch-icon.png
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `In the absence of an explicitly specified caching lifetime directive in the response, a liberal lifetime heuristic of 1 year was assumed. This is permitted by rfc7234.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/favicon.ico
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `In the absence of an explicitly specified caching lifetime directive in the response, a liberal lifetime heuristic of 1 year was assumed. This is permitted by rfc7234.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/robots.txt
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `In the absence of an explicitly specified caching lifetime directive in the response, a liberal lifetime heuristic of 1 year was assumed. This is permitted by rfc7234.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/sitemap.xml
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `In the absence of an explicitly specified caching lifetime directive in the response, a liberal lifetime heuristic of 1 year was assumed. This is permitted by rfc7234.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/styles/fonts.css
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `In the absence of an explicitly specified caching lifetime directive in the response, a liberal lifetime heuristic of 1 year was assumed. This is permitted by rfc7234.`

Instances: 7

### Solution

Validate that the response does not contain sensitive, personal or user-specific information.  If it does, consider the use of the following HTTP response headers, to limit, or prevent the content being stored and retrieved from the cache by another user:
Cache-Control: no-cache, no-store, must-revalidate, private
Pragma: no-cache
Expires: 0
This configuration directs both HTTP 1.0 and HTTP 1.1 compliant caching servers to not store the response, and to not retrieve the response (without validation) from the cache, in response to a similar request. 

### Reference


* [ https://datatracker.ietf.org/doc/html/rfc7234 ](https://datatracker.ietf.org/doc/html/rfc7234)
* [ https://datatracker.ietf.org/doc/html/rfc7231 ](https://datatracker.ietf.org/doc/html/rfc7231)
* [ https://www.w3.org/Protocols/rfc2616/rfc2616-sec13.html ](https://www.w3.org/Protocols/rfc2616/rfc2616-sec13.html)


#### CWE Id: [ 524 ](https://cwe.mitre.org/data/definitions/524.html)


#### WASC Id: 13

#### Source ID: 3


