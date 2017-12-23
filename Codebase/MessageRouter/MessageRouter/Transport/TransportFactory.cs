﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageRouter.Addresses;
using MessageRouter.Monitors;
using MessageRouter.Publishers;
using MessageRouter.Receivers;
using MessageRouter.Senders;
using MessageRouter.Subscribers;

namespace MessageRouter.Transport
{
    /// <summary>
    /// Abstract implementation of <see cref="ITransportFactory{TSender, TReceiver, TPublisher, TSubscriber}"/> with common methods implemented for convienence
    /// </summary>
    /// <typeparam name="TSender">The transport specific implementation of <see cref="ISender"/> this factory creates</typeparam>
    /// <typeparam name="TReceiver">The transport specific implementation of <see cref="IReceiver"/> this factory creates</typeparam>
    /// <typeparam name="TPublisher">The transport specific implementation of <see cref="IPublisher"/> this factory creates</typeparam>
    /// <typeparam name="TSubscriber">The transport specific implementation of <see cref="ISubscriber"/> this factory creates</typeparam>
    public abstract class TransportFactory<TSender, TReceiver, TPublisher, TSubscriber> : ITransportFactory<TSender, TReceiver, TPublisher, TSubscriber>
        where TSender : ISender
        where TReceiver : IReceiver
        where TPublisher : IPublisher
        where TSubscriber : ISubscriber
    {
        private readonly ISenderMonitor<TSender> senderMonitor;
        private readonly IReceiverMonitor<TReceiver> receiverMonitor;
        private readonly IPublisherMonitor<TPublisher> publisherMonitor;
        private readonly ISubscriberMonitor<TSubscriber> subscriberMonitor;


        #region Properties
        /// <summary>
        /// Gets the <see cref="IMonitor"/> for <see cref="TSender"/>s
        /// </summary>
        public IMonitor SenderMonitor => senderMonitor;


        /// <summary>
        /// Gets the <see cref="IMonitor"/> for <see cref="TReceiver"/>s
        /// </summary>
        public IMonitor ReceiverMonitor => receiverMonitor;


        /// <summary>
        /// Gets the <see cref="IMonitor"/> for <see cref="TPublisher"/>s
        /// </summary>
        public IMonitor PublisherMonitor => publisherMonitor;


        /// <summary>
        /// Gets the <see cref="IMonitor"/> for <see cref="ISubscriber"/>s
        /// </summary>
        public IMonitor SubscriberMonitor => subscriberMonitor;


        /// <summary>
        /// Gets the type of <see cref="ISender"/>s this factory creates
        /// </summary>
        public Type SenderType => typeof(TSender);


        /// <summary>
        /// Gets the type of <see cref="IReceiver"/>s this factory creates
        /// </summary>
        public Type ReceiverType => typeof(TReceiver);


        /// <summary>
        /// Gets the type of <see cref="IPublisher"/>s this factory creates
        /// </summary>
        public Type PublisherType => typeof(TPublisher);


        /// <summary>
        /// Gets the type of <see cref="ISubscriber"/>s this factory creates
        /// </summary>
        public Type SubscriberType => typeof(TSubscriber);
        #endregion


        /// <summary>
        /// Initializes a new instance of <see cref="TransportFactory{TSender, TReceiver, TPublisher, TSubscriber}"/>
        /// </summary>
        /// <param name="senderMonitor">Monitor that <see cref="TSender"/>s will be added to</param>
        /// <param name="receiverMonitor">Monitor that <see cref="TReceiver"/>s will be added to</param>
        public TransportFactory(
            ISenderMonitor<TSender> senderMonitor, 
            IReceiverMonitor<TReceiver> receiverMonitor, 
            IPublisherMonitor<TPublisher> publisherMonitor, 
            ISubscriberMonitor<TSubscriber> subscriberMonitor)
        {
            this.senderMonitor = senderMonitor;
            this.receiverMonitor = receiverMonitor;
            this.publisherMonitor = publisherMonitor;
            this.subscriberMonitor = subscriberMonitor;
        }


        #region Senders
        /// <summary>
        /// Constructs a new instance of an <see cref="ISender"/> connected to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of the remote the sender will connect to</param>
        /// <returns>Sender connected to the remote address</returns>
        public ISender CreateSender(IAddress address)
        {
            return CreateAndAddSender(address);
        }
        

        /// <summary>
        /// Constructs a new instance of an <see cref="TSender"/> connected to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of the remote the sender will connect to</param>
        /// <returns>Sender connected to the remote address</returns>
        protected abstract TSender CreateNewSender(IAddress address);


        private TSender CreateAndAddSender(IAddress address)
        {
            var sender = CreateNewSender(address);
            senderMonitor.AddSender(sender);
            return sender;
        }
        #endregion


        #region Receivers
        /// <summary>
        /// Creates a new instance of a <see cref="IReceiver"/> bound to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of local bound <see cref="Common.IConnection"/></param>
        /// <param name="requestTaskHandler"><see cref="RequestTaskHandler"/> delegate that is called when by the 
        /// <see cref="IReceiver"/> when an incoming message is received</param>
        /// <returns>Receiver bound to the address</returns>
        public IReceiver CreateReceiver(IAddress address, RequestTaskHandler requestTaskHandler)
        {
            return CreateAndAddReceiver(address, requestTaskHandler);
        }


        /// <summary>
        /// Creates a new instance of a <see cref="TReceiver"/> bound to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address">Address of local bound <see cref="Common.IConnection"/></param>
        /// <param name="requestTaskHandler"><see cref="RequestTaskHandler"/> delegate that is called when by the 
        /// <see cref="IReceiver"/> when an incoming message is received</param>
        /// <returns>Receiver bound to the address</returns>
        protected abstract TReceiver CreateNewReceiver(IAddress address, RequestTaskHandler requestTaskHandler);


        private TReceiver CreateAndAddReceiver(IAddress address, RequestTaskHandler requestTaskHandler)
        {
            var receiver = CreateNewReceiver(address, requestTaskHandler);
            receiverMonitor.AddReceiver(receiver);
            return receiver;
        }
        #endregion


        #region Publishers
        /// <summary>
        /// Creates a new instance of a <see cref="IPublisher"/> bound to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> publisher binds to</param>
        /// <returns><see cref="IPublisher"/> bound to the <see cref="IAddress"/></returns>
        public IPublisher CreatePublisher(IAddress address)
        {
            return CreateAndAddPublisher(address);
        }


        /// <summary>
        /// Creates a new instance of a <see cref="TPublisher"/> bound to the supplied <see cref="IAddress"/>
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> publisher binds to</param>
        /// <returns><see cref="TPublisher"/> bound to the <see cref="IAddress"/></returns>
        protected abstract TPublisher CreateNewPublisher(IAddress address);


        private TPublisher CreateAndAddPublisher(IAddress address)
        {
            var publisher = CreateNewPublisher(address);
            publisherMonitor.AddPublisher(publisher);
            return publisher;
        }
        #endregion


        #region Subscribers
        /// <summary>
        /// Creates a new instance of a <see cref="ISubscriber"/> connected to the supplied <see cref="IAddress"/> and
        /// monitored by the factories <see cref="IMonitor"/>
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> of the remote publishing <see cref="Common.IConnection"/></param>
        /// <param name="topicEventHandler"><see cref="TopicEventHandler"/> delegate that the <see cref="ISubscriber"/> will
        /// call upon receiving a new topic message</param>
        /// <returns><see cref="ISubscriber"/> connected to the <see cref="IAddress"/></returns>
        public ISubscriber CreateSubscriber(IAddress address, TopicEventHandler topicEventHandler)
        {
            return CreateAndAddSubscriber(address, topicEventHandler);
        }


        /// <summary>
        /// Creates a new instance of a <see cref="TSubscriber"/> connected to the supplied <see cref="IAddress"/> and
        /// monitored by the factories <see cref="IMonitor"/>
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> of the remote publishing <see cref="Common.IConnection"/></param>
        /// <param name="topicEventHandler"><see cref="TopicEventHandler"/> delegate that the <see cref="ISubscriber"/> will
        /// call upon receiving a new topic message</param>
        /// <returns><see cref="TSubscriber"/> connected to the <see cref="IAddress"/></returns>
        protected abstract TSubscriber CreateNewSubscriber(IAddress address, TopicEventHandler topicEventHandler);


        private TSubscriber CreateAndAddSubscriber(IAddress address, TopicEventHandler topicEventHandler)
        {
            var subscriber = CreateNewSubscriber(address, topicEventHandler);
            subscriberMonitor.AddSubscriber(subscriber);
            return subscriber;
        }
        #endregion
    }
}
