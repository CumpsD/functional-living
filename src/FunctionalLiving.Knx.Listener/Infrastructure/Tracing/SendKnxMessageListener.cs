//namespace FunctionalLiving.Knx.Listener.Infrastructure.Tracing
//{
//    using System.Diagnostics;
//    using OpenTelemetry.Adapter;
//    using OpenTelemetry.Trace;

//    public class SendKnxMessageListener : ListenerHandler
//    {
//        public SendKnxMessageListener(string sourceName, Tracer tracer)
//            : base(sourceName, tracer) { }

//        public override void OnStartActivity(Activity activity, object payload)
//            => ProcessEvent(activity, payload as BeforeSendMessage);

//        public override void OnStopActivity(Activity activity, object payload)
//            => Tracer.CurrentSpan.End();

//        private void ProcessEvent(Activity activity, BeforeSendMessage? payload)
//        {
//            if (payload == null)
//            {
//                AdapterEventSource.Log.NullPayload("SendMessageListener.OnStartActivity");
//                return;
//            }

//            Tracer.StartActiveSpanFromActivity(
//                activity.OperationName,
//                activity,
//                SpanKind.Producer,
//                out _);
//        }
//    }
//}
