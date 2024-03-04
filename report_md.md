# ZAP Scanning Report


## Summary of Alerts

| Risk Level | Number of Alerts |
| --- | --- |
| High | 0 |
| Medium | 6 |
| Low | 2 |
| Informational | 9 |




## Alerts

| Name | Risk Level | Number of Instances |
| --- | --- | --- |
| CSP: script-src unsafe-eval | Medium | 1 |
| CSP: script-src unsafe-inline | Medium | 1 |
| CSP: style-src unsafe-inline | Medium | 1 |
| Hidden File Found | Medium | 1 |
| Proxy Disclosure | Medium | 11 |
| Relative Path Confusion | Medium | 7 |
| Permissions Policy Header Not Set | Low | 2 |
| Timestamp Disclosure - Unix | Low | 1 |
| Base64 Disclosure | Informational | 2 |
| Information Disclosure - Suspicious Comments | Informational | 4 |
| Modern Web Application | Informational | 1 |
| Re-examine Cache-control Directives | Informational | 1 |
| Sec-Fetch-Dest Header is Missing | Informational | 3 |
| Sec-Fetch-Mode Header is Missing | Informational | 3 |
| Sec-Fetch-Site Header is Missing | Informational | 3 |
| Sec-Fetch-User Header is Missing | Informational | 3 |
| Storable and Cacheable Content | Informational | 8 |




## Alert Detail



### [ CSP: script-src unsafe-eval ](https://www.zaproxy.org/docs/alerts/10055/)



##### Medium (High)

### Description

Content Security Policy (CSP) is an added layer of security that helps to detect and mitigate certain types of attacks. Including (but not limited to) Cross Site Scripting (XSS), and data injection attacks. These attacks are used for everything from data theft to site defacement or distribution of malware. CSP provides a set of standard HTTP headers that allow website owners to declare approved sources of content that browsers should be allowed to load on that page — covered types are JavaScript, CSS, HTML frames, fonts, images and embeddable objects such as Java applets, ActiveX, audio and video files.

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/
  * Method: `GET`
  * Parameter: `content-security-policy`
  * Attack: ``
  * Evidence: `base-uri 'self';default-src 'self';script-src 'self' 'unsafe-inline' 'unsafe-eval';connect-src 'self' https://dev.loginproxy.gov.bc.ca/ https://idtest.gov.bc.ca/;img-src 'self' data:;style-src 'self' 'unsafe-inline';frame-ancestors 'self';form-action 'self';`
  * Other Info: `script-src includes unsafe-eval.`

Instances: 1

### Solution

Ensure that your web server, application server, load balancer, etc. is properly configured to set the Content-Security-Policy header.

### Reference


* [ https://www.w3.org/TR/CSP/ ](https://www.w3.org/TR/CSP/)
* [ https://caniuse.com/#search=content+security+policy ](https://caniuse.com/#search=content+security+policy)
* [ https://content-security-policy.com/ ](https://content-security-policy.com/)
* [ https://github.com/HtmlUnit/htmlunit-csp ](https://github.com/HtmlUnit/htmlunit-csp)
* [ https://developers.google.com/web/fundamentals/security/csp#policy_applies_to_a_wide_variety_of_resources ](https://developers.google.com/web/fundamentals/security/csp#policy_applies_to_a_wide_variety_of_resources)


#### CWE Id: [ 693 ](https://cwe.mitre.org/data/definitions/693.html)


#### WASC Id: 15

#### Source ID: 3

### [ CSP: script-src unsafe-inline ](https://www.zaproxy.org/docs/alerts/10055/)



##### Medium (High)

### Description

Content Security Policy (CSP) is an added layer of security that helps to detect and mitigate certain types of attacks. Including (but not limited to) Cross Site Scripting (XSS), and data injection attacks. These attacks are used for everything from data theft to site defacement or distribution of malware. CSP provides a set of standard HTTP headers that allow website owners to declare approved sources of content that browsers should be allowed to load on that page — covered types are JavaScript, CSS, HTML frames, fonts, images and embeddable objects such as Java applets, ActiveX, audio and video files.

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/
  * Method: `GET`
  * Parameter: `content-security-policy`
  * Attack: ``
  * Evidence: `base-uri 'self';default-src 'self';script-src 'self' 'unsafe-inline' 'unsafe-eval';connect-src 'self' https://dev.loginproxy.gov.bc.ca/ https://idtest.gov.bc.ca/;img-src 'self' data:;style-src 'self' 'unsafe-inline';frame-ancestors 'self';form-action 'self';`
  * Other Info: `script-src includes unsafe-inline.`

Instances: 1

### Solution

Ensure that your web server, application server, load balancer, etc. is properly configured to set the Content-Security-Policy header.

### Reference


* [ https://www.w3.org/TR/CSP/ ](https://www.w3.org/TR/CSP/)
* [ https://caniuse.com/#search=content+security+policy ](https://caniuse.com/#search=content+security+policy)
* [ https://content-security-policy.com/ ](https://content-security-policy.com/)
* [ https://github.com/HtmlUnit/htmlunit-csp ](https://github.com/HtmlUnit/htmlunit-csp)
* [ https://developers.google.com/web/fundamentals/security/csp#policy_applies_to_a_wide_variety_of_resources ](https://developers.google.com/web/fundamentals/security/csp#policy_applies_to_a_wide_variety_of_resources)


#### CWE Id: [ 693 ](https://cwe.mitre.org/data/definitions/693.html)


#### WASC Id: 15

#### Source ID: 3

### [ CSP: style-src unsafe-inline ](https://www.zaproxy.org/docs/alerts/10055/)



##### Medium (High)

### Description

Content Security Policy (CSP) is an added layer of security that helps to detect and mitigate certain types of attacks. Including (but not limited to) Cross Site Scripting (XSS), and data injection attacks. These attacks are used for everything from data theft to site defacement or distribution of malware. CSP provides a set of standard HTTP headers that allow website owners to declare approved sources of content that browsers should be allowed to load on that page — covered types are JavaScript, CSS, HTML frames, fonts, images and embeddable objects such as Java applets, ActiveX, audio and video files.

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/
  * Method: `GET`
  * Parameter: `content-security-policy`
  * Attack: ``
  * Evidence: `base-uri 'self';default-src 'self';script-src 'self' 'unsafe-inline' 'unsafe-eval';connect-src 'self' https://dev.loginproxy.gov.bc.ca/ https://idtest.gov.bc.ca/;img-src 'self' data:;style-src 'self' 'unsafe-inline';frame-ancestors 'self';form-action 'self';`
  * Other Info: `style-src includes unsafe-inline.`

Instances: 1

### Solution

Ensure that your web server, application server, load balancer, etc. is properly configured to set the Content-Security-Policy header.

### Reference


* [ https://www.w3.org/TR/CSP/ ](https://www.w3.org/TR/CSP/)
* [ https://caniuse.com/#search=content+security+policy ](https://caniuse.com/#search=content+security+policy)
* [ https://content-security-policy.com/ ](https://content-security-policy.com/)
* [ https://github.com/HtmlUnit/htmlunit-csp ](https://github.com/HtmlUnit/htmlunit-csp)
* [ https://developers.google.com/web/fundamentals/security/csp#policy_applies_to_a_wide_variety_of_resources ](https://developers.google.com/web/fundamentals/security/csp#policy_applies_to_a_wide_variety_of_resources)


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
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-6121be92.js
  * Method: `GET`
  * Parameter: ``
  * Attack: `TRACE, OPTIONS methods with 'Max-Forwards' header. TRACK method.`
  * Evidence: ``
  * Other Info: `Using the TRACE, OPTIONS, and TRACK methods, the following proxy servers have been identified between ZAP and the application/web server: 
- Unknown
The following web/application server has been identified: 
- Kestrel
`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-b8868db3.css
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

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-6121be92.js
  * Method: `GET`
  * Parameter: ``
  * Attack: `https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-6121be92.js/rocp5/vzs06`
  * Evidence: `<link rel="apple-touch-icon" href="bcid-apple-touch-icon.png">`
  * Other Info: `No <base> tag was specified in the HTML <head> tag to define the location for relative URLs.
A Content Type of "text/html" was specified. If the web browser is employing strict parsing rules, this will prevent cross-content attacks from succeeding. Quirks Mode in the web browser would disable strict parsing.  
No X-Frame-Options header was specified, so the page can be framed, and this can be used to enable Quirks Mode, allowing the specified Content Type to be bypassed.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-b8868db3.css
  * Method: `GET`
  * Parameter: ``
  * Attack: `https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-b8868db3.css/rocp5/vzs06`
  * Evidence: `<link rel="apple-touch-icon" href="bcid-apple-touch-icon.png">`
  * Other Info: `No <base> tag was specified in the HTML <head> tag to define the location for relative URLs.
A Content Type of "text/html" was specified. If the web browser is employing strict parsing rules, this will prevent cross-content attacks from succeeding. Quirks Mode in the web browser would disable strict parsing.  
No X-Frame-Options header was specified, so the page can be framed, and this can be used to enable Quirks Mode, allowing the specified Content Type to be bypassed.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/bcid-apple-touch-icon.png
  * Method: `GET`
  * Parameter: ``
  * Attack: `https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/bcid-apple-touch-icon.png/rocp5/vzs06`
  * Evidence: `<link rel="apple-touch-icon" href="bcid-apple-touch-icon.png">`
  * Other Info: `No <base> tag was specified in the HTML <head> tag to define the location for relative URLs.
A Content Type of "text/html" was specified. If the web browser is employing strict parsing rules, this will prevent cross-content attacks from succeeding. Quirks Mode in the web browser would disable strict parsing.  
No X-Frame-Options header was specified, so the page can be framed, and this can be used to enable Quirks Mode, allowing the specified Content Type to be bypassed.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/favicon.ico
  * Method: `GET`
  * Parameter: ``
  * Attack: `https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/favicon.ico/rocp5/vzs06`
  * Evidence: `<link rel="apple-touch-icon" href="bcid-apple-touch-icon.png">`
  * Other Info: `No <base> tag was specified in the HTML <head> tag to define the location for relative URLs.
A Content Type of "text/html" was specified. If the web browser is employing strict parsing rules, this will prevent cross-content attacks from succeeding. Quirks Mode in the web browser would disable strict parsing.  
No X-Frame-Options header was specified, so the page can be framed, and this can be used to enable Quirks Mode, allowing the specified Content Type to be bypassed.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/robots.txt
  * Method: `GET`
  * Parameter: ``
  * Attack: `https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/robots.txt/rocp5/vzs06`
  * Evidence: `<link rel="apple-touch-icon" href="bcid-apple-touch-icon.png">`
  * Other Info: `No <base> tag was specified in the HTML <head> tag to define the location for relative URLs.
A Content Type of "text/html" was specified. If the web browser is employing strict parsing rules, this will prevent cross-content attacks from succeeding. Quirks Mode in the web browser would disable strict parsing.  
No X-Frame-Options header was specified, so the page can be framed, and this can be used to enable Quirks Mode, allowing the specified Content Type to be bypassed.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/sitemap.xml
  * Method: `GET`
  * Parameter: ``
  * Attack: `https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/sitemap.xml/rocp5/vzs06`
  * Evidence: `<link rel="apple-touch-icon" href="bcid-apple-touch-icon.png">`
  * Other Info: `No <base> tag was specified in the HTML <head> tag to define the location for relative URLs.
A Content Type of "text/html" was specified. If the web browser is employing strict parsing rules, this will prevent cross-content attacks from succeeding. Quirks Mode in the web browser would disable strict parsing.  
No X-Frame-Options header was specified, so the page can be framed, and this can be used to enable Quirks Mode, allowing the specified Content Type to be bypassed.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/styles/fonts.css
  * Method: `GET`
  * Parameter: ``
  * Attack: `https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/styles/fonts.css/rocp5/vzs06`
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


* [ https://arxiv.org/abs/1811.00917 ](https://arxiv.org/abs/1811.00917)
* [ https://hsivonen.fi/doctype/ ](https://hsivonen.fi/doctype/)
* [ https://www.w3schools.com/tags/tag_base.asp ](https://www.w3schools.com/tags/tag_base.asp)


#### CWE Id: [ 20 ](https://cwe.mitre.org/data/definitions/20.html)


#### WASC Id: 20

#### Source ID: 1

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
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-6121be92.js
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

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-b8868db3.css
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `1428571429`
  * Other Info: `1428571429, which evaluates to: 2015-04-09 09:23:49`

Instances: 1

### Solution

Manually confirm that the timestamp data is not sensitive, and that the data cannot be aggregated to disclose exploitable patterns.

### Reference


* [ https://cwe.mitre.org/data/definitions/200.html ](https://cwe.mitre.org/data/definitions/200.html)


#### CWE Id: [ 200 ](https://cwe.mitre.org/data/definitions/200.html)


#### WASC Id: 13

#### Source ID: 3

### [ Base64 Disclosure ](https://www.zaproxy.org/docs/alerts/10094/)



##### Informational (Medium)

### Description

Base64 encoded data was disclosed by the application/web server. Note: in the interests of performance not all base64 strings in the response were analyzed individually, the entire response should be looked at by the analyst/security team/developer(s).

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-6121be92.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `assets/Certifications-b125eb4c`
  * Other Info: `j����z�b~'�*'���ۗ��`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-b8868db3.css
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `iVBORw0KGgoAAAANSUhEUgAAAAgAAAAICAYAAADED76LAAAAAXNSR0IArs4c6QAAACRJREFUKFNjPHTo0H8GJGBnZ8eIzGekgwJk+0BsdCtRHEQbBQBbbh0dIGKknQAAAABJRU5ErkJggg==`
  * Other Info: `�PNG

   IHDR         ���   sRGB ���   $IDAT(Sc<t��$`ggǈ�g��d�@lt+QD [n b��    IEND�B`�`

Instances: 2

### Solution

Manually confirm that the Base64 data does not leak sensitive information, and that the data cannot be aggregated/used to exploit other vulnerabilities.

### Reference


* [ https://projects.webappsec.org/w/page/13246936/Information%20Leakage ](https://projects.webappsec.org/w/page/13246936/Information%20Leakage)


#### CWE Id: [ 200 ](https://cwe.mitre.org/data/definitions/200.html)


#### WASC Id: 13

#### Source ID: 3

### [ Information Disclosure - Suspicious Comments ](https://www.zaproxy.org/docs/alerts/10027/)



##### Informational (Low)

### Description

The response appears to contain suspicious comments which may help an attacker. Note: Matches made within script blocks or files are against the entire content not only comments.

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-6121be92.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `from`
  * Other Info: `The following pattern was used: \bFROM\b and was detected 2 times, the first in the element starting with: "**/function kr(e,t,n,r){let i;try{i=r?e(...r):e()}catch(a){Da(a,t,n)}return i}function pn(e,t,n,r){if(Ue(e)){const a=kr(e,t,n,r)", see evidence field for the suspicious comment/snippet.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-6121be92.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `query`
  * Other Info: `The following pattern was used: \bQUERY\b and was detected 4 times, the first in the element starting with: "`)}function kv(e){const t=e.dark?2:1,n=e.dark?1:2,r=[];for(const[i,a]of Object.entries(e.colors)){const o=gn(a);r.push(`--v-them", see evidence field for the suspicious comment/snippet.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-6121be92.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `select`
  * Other Info: `The following pattern was used: \bSELECT\b and was detected 3 times, the first in the element starting with: "**/const BS="http://www.w3.org/2000/svg",MS="http://www.w3.org/1998/Math/MathML",br=typeof document<"u"?document:null,Cf=br&&br.", see evidence field for the suspicious comment/snippet.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-6121be92.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: `User`
  * Other Info: `The following pattern was used: \bUSER\b and was detected in the element starting with: "`)}get[Symbol.toStringTag](){return"AxiosHeaders"}static from(t){return t instanceof this?t:new this(t)}static concat(t,...n){co", see evidence field for the suspicious comment/snippet.`

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
  * Evidence: `<script type="module" crossorigin src="/assets/index-6121be92.js"></script>`
  * Other Info: `No links have been found while there are scripts, which is an indication that this is a modern web application.`

Instances: 1

### Solution

This is an informational alert and so no changes are required.

### Reference




#### Source ID: 3

### [ Re-examine Cache-control Directives ](https://www.zaproxy.org/docs/alerts/10015/)



##### Informational (Low)

### Description

The cache-control header has not been set properly or is missing, allowing the browser and proxies to cache content. For static assets like css, js, or image files this might be intended, however, the resources should be reviewed to ensure that no sensitive content will be cached.

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/
  * Method: `GET`
  * Parameter: `cache-control`
  * Attack: ``
  * Evidence: ``
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
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/robots.txt
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

Instances: 3

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
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/robots.txt
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

Instances: 3

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
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/robots.txt
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

Instances: 3

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
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/robots.txt
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

Instances: 3

### Solution

Ensure that Sec-Fetch-User header is included in user initiated requests.

### Reference


* [ https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Sec-Fetch-User ](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Sec-Fetch-User)


#### CWE Id: [ 352 ](https://cwe.mitre.org/data/definitions/352.html)


#### WASC Id: 9

#### Source ID: 3

### [ Storable and Cacheable Content ](https://www.zaproxy.org/docs/alerts/10049/)



##### Informational (Medium)

### Description

The response contents are storable by caching components such as proxy servers, and may be retrieved directly from the cache, rather than from the origin server by the caching servers, in response to similar requests from other users.  If the response data is sensitive, personal or user-specific, this may result in sensitive information being leaked. In some cases, this may even result in a user gaining complete control of the session of another user, depending on the configuration of the caching components in use in their environment. This is primarily an issue where "shared" caching servers such as "proxy" caches are configured on the local network. This configuration is typically found in corporate or educational environments, for instance.

* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `In the absence of an explicitly specified caching lifetime directive in the response, a liberal lifetime heuristic of 1 year was assumed. This is permitted by rfc7234.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-6121be92.js
  * Method: `GET`
  * Parameter: ``
  * Attack: ``
  * Evidence: ``
  * Other Info: `In the absence of an explicitly specified caching lifetime directive in the response, a liberal lifetime heuristic of 1 year was assumed. This is permitted by rfc7234.`
* URL: https://dev-ecer-registry-portal.apps.silver.devops.gov.bc.ca/assets/index-b8868db3.css
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

Instances: 8

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


