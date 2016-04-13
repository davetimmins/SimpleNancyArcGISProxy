# SimpleNancyArcGISProxy
A simple proxy for ArcGIS Server in Nancy :open_mouth:


_Hopefully this is obvious but this technique should only be used as a last resort since circumventing the security measures of your web browser like this can have potential risks if adding data from unknown / uncontrolled sources. So if you can, use HTTPS everywhere, if you can't then this is a 'hack', 'workaround', 'solution' whatever you want to call it. Just know that it is not the best approach :imp:_

If your site runs under SSL but you __need__ to add services using http only then this will do the trick, and thats about it at the moment

Using the ArcGIS JS API v3.16 https://simple-nancy-proxy.azurewebsites.net/v3.html

Using the ArcGIS JS API v4 (not final version yet) https://simple-nancy-proxy.azurewebsites.net/v4.html

Possible usage

```js
let layer = ArcGISDynamicLayer.fromJSON(...)     
if (location.protocol === 'https:' && layer.url.toLowerCase().startsWith('http:')) {
    urlUtils.addProxyRule({ proxyUrl: '/proxy/', urlPrefix: layer.url })
}
```
