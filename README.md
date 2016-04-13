# SimpleNancyArcGISProxy
A simple proxy for ArcGIS Server in Nancy :open_mouth:

If your site runs under SSL but you need to add services using http only then this will do the trick, and thats about it at the moment

Using the ArcGIS JS API v3.16 https://simple-nancy-proxy.azurewebsites.net/v3.html

Using the ArcGIS JS API v4 (not final version yet) https://simple-nancy-proxy.azurewebsites.net/v4.html

Possible usage

```js
let layer = ArcGISDynamicLayer.fromJSON(...)     
if (location.protocol === 'https:' && layer.url.toLowerCase().startsWith('http:')) {
    urlUtils.addProxyRule({ proxyUrl: '/proxy/', urlPrefix: layer.url })
}
```
