import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-http';
import { WebTracerProvider, BatchSpanProcessor } from '@opentelemetry/sdk-trace-web';
import { ZoneContextManager } from '@opentelemetry/context-zone';
import { Resource }  from '@opentelemetry/resources';
import { SemanticResourceAttributes } from '@opentelemetry/semantic-conventions';
import { registerInstrumentations } from '@opentelemetry/instrumentation';
import { FetchInstrumentation } from '@opentelemetry/instrumentation-fetch';
import { getWebAutoInstrumentations } from '@opentelemetry/auto-instrumentations-web';
import { DocumentLoadInstrumentation } from '@opentelemetry/instrumentation-document-load';
import { UserInteractionInstrumentation } from '@opentelemetry/instrumentation-user-interaction';
import { LongTaskInstrumentation } from '@opentelemetry/instrumentation-long-task';

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
     },
     // load custom configuration for fetch instrumentation
     '@opentelemetry/instrumentation-fetch': {
       propagateTraceHeaderCorsUrls: [
           /.+api.lvh.me.+/g,
           /.+localhost:5293.+/g,
         ],
     },
   }),
    new DocumentLoadInstrumentation(),
    new UserInteractionInstrumentation({
        eventNames: ['submit', 'click', 'keypress'],
      }),
    new LongTaskInstrumentation({
        observerCallback: (span, longtaskEvent) => {
          span.setAttribute('location.pathname', window.location.pathname)
        }
      }),
  ],
});
