import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-http';
import { WebTracerProvider, BatchSpanProcessor } from '@opentelemetry/sdk-trace-web';
import { ZoneContextManager } from '@opentelemetry/context-zone';
import { Resource }  from '@opentelemetry/resources';
import { SemanticResourceAttributes } from '@opentelemetry/semantic-conventions';
import { registerInstrumentations } from '@opentelemetry/instrumentation';
import { getWebAutoInstrumentations } from '@opentelemetry/auto-instrumentations-web';
import { DocumentLoadInstrumentation } from '@opentelemetry/instrumentation-document-load';
import { UserInteractionInstrumentation } from '@opentelemetry/instrumentation-user-interaction';
import { LongTaskInstrumentation } from '@opentelemetry/instrumentation-long-task';

// The exporter is responsible for sending traces from the browser to your collector
const exporter = new OTLPTraceExporter({
    url: "https://api.honeycomb.io/v1/traces",
    headers: {
      "x-honeycomb-team": "B8D6vMhN5gF0K2PAq8tLjA",
    }
  })
// The TracerProvider is the core library for creating traces
const provider = new WebTracerProvider({
  resource: new Resource({
    [SemanticResourceAttributes.SERVICE_NAME]: 'react-app',
  }),
});
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
    getWebAutoInstrumentations(),
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
