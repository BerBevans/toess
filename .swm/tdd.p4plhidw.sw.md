---
title: TDD
---
<SwmSnippet path="/src/toess-react/src/tracing.js" line="13">

---

<SwmPath>[src/toess-react/src/tracing.js](/src/toess-react/src/tracing.js)</SwmPath> is used to integrate OpenTelemetry traceing.

```javascript
// The exporter is responsible for sending traces from the browser to your collector
const exporter = new OTLPTraceExporter({
    url: 'http://localhost:4318/v1/traces',
  })

// The TracerProvider is the core library for creating traces
const provider = new WebTracerProvider({
  resource: new Resource({
    [SemanticResourceAttributes.SERVICE_NAME]: 'toess-react',
  }),
});

const fetchInstrumentation = new FetchInstrumentation({});
fetchInstrumentation.setTracerProvider(provider);

// The processor sorts through data as it comes in, before it is sent to the exporter
provider.addSpanProcessor(new BatchSpanProcessor(exporter));

// A context manager allows OTel to keep the context of function calls across async functions
// ensuring you don't have disconnected traces
provider.register({
  contextManager: new ZoneContextManager()
});

// 
registerInstrumentations({
  instrumentations: [
    getWebAutoInstrumentations({
     // load custom configuration for xml-http-request instrumentation
     '@opentelemetry/instrumentation-xml-http-request': {
       propagateTraceHeaderCorsUrls: [
           /.+api.lvh.me.+/g,
           /.+localhost:5293.+/g,
         ],
```

---

</SwmSnippet>

<SwmMeta version="3.0.0" repo-id="Z2l0aHViJTNBJTNBdG9lc3MlM0ElM0FCZXJCZXZhbnM=" repo-name="toess"><sup>Powered by [Swimm](https://app.swimm.io/)</sup></SwmMeta>
