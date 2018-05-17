﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GSF.Diagnostics;
using GSF.Threading;
using GSF.TimeSeries.Transport;
using Sttp.Adapters;
using Sttp.Services;

namespace Sttp.Adapter.Test
{
    static class Program
    {
        private static SttpPublisher m_publisher;
        private static AutoResetEvent m_wait = new AutoResetEvent(false);


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Logger.Console.Verbose = VerboseLevel.All;
            Logger.FileWriter.SetPath(@"C:\temp\SttpLogs");
            Logger.FileWriter.Verbose = VerboseLevel.All;
            var subscriber = new DataSubscriber();
            subscriber.StatusMessage += Subscriber_StatusMessage;
            subscriber.MetaDataReceived += SubscriberOnMetaDataReceived;
            subscriber.ProcessException += SubscriberOnProcessException;
            subscriber.NewMeasurements += Subscriber_NewMeasurements;
            subscriber.ConnectionString = "server=phasor2:6170";
            subscriber.OperationalModes |= OperationalModes.UseCommonSerializationFormat | OperationalModes.CompressMetadata | OperationalModes.CompressSignalIndexCache | OperationalModes.CompressPayloadData;
            subscriber.CompressionModes = CompressionModes.TSSC | CompressionModes.GZip;
            subscriber.Initialize();
            subscriber.Start();
            subscriber.MeasurementReportingInterval = 1_000_000;
            subscriber.ConnectionEstablished += Subscriber_ConnectionEstablished;
            m_wait.WaitOne();
            Thread.Sleep(1000);

            subscriber.RefreshMetadata();
            var si = new UnsynchronizedSubscriptionInfo(false);
            si.FilterExpression = "FILTER ActiveMeasurements WHERE ID IS NOT NULL";
            subscriber.Subscribe(si);

            m_wait.WaitOne();

            var pub = new SttpPublisher();
            pub.DataSource = m_metadata;
            pub.Initialize();
            pub.Start();
            m_publisher = pub;

            Console.ReadLine();
            pub.Stop();
            subscriber.Unsubscribe();
            subscriber.Stop();
            Console.ReadLine();
        }

        private static void Subscriber_ConnectionEstablished(object sender, EventArgs e)
        {
            m_wait.Set();
        }

        private static DataSet m_metadata;
        private static RateLimiter m_rate = new RateLimiter(1, 1);

        private static void Subscriber_NewMeasurements(object sender, GSF.EventArgs<ICollection<GSF.TimeSeries.IMeasurement>> e)
        {
            if (m_rate.TryTakeToken())
                Console.WriteLine($"Measurements Received: " + e.Argument.Count);
            m_publisher?.QueueMeasurementsForProcessing(e.Argument);
        }

        private static void SubscriberOnProcessException(object sender, GSF.EventArgs<Exception> eventArgs)
        {
            Console.WriteLine($"Exception: " + eventArgs.Argument.ToString());
        }

        private static void SubscriberOnMetaDataReceived(object sender, GSF.EventArgs<DataSet> eventArgs)
        {
            m_metadata = eventArgs.Argument;
            Console.WriteLine($"Metadata Received");
            m_wait.Set();
        }

        private static void Subscriber_StatusMessage(object sender, GSF.EventArgs<string> e)
        {
            Console.WriteLine($"Status: " + e.Argument.ToString());
        }
    }
}