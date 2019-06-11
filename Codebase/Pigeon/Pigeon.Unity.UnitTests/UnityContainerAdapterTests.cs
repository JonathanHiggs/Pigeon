using System;

using Moq;

using NUnit.Framework;

using Pigeon.Unity.UnitTests.TestFixtures;

using Unity;
using Unity.Lifetime;
using Unity.Resolution;

namespace Pigeon.Unity.UnitTests
{
    [TestFixture]
    public class UnityContainerAdapterTests
    {
        private readonly Mock<IUnityContainer> mockUnityContainer = new Mock<IUnityContainer>();
        private IUnityContainer unityContainer;


        [SetUp]
        public void Setup()
        {
            unityContainer = mockUnityContainer.Object;

            mockUnityContainer
                .Setup(
                    m => m.Resolve(
                        It.IsIn(typeof(Service)), 
                        It.IsAny<string>(), 
                        It.IsAny<ResolverOverride[]>()))
                .Returns(new Service());
        }


        [TearDown]
        public void TearDown()
        {
            mockUnityContainer.Reset();
        }


        [Test]
        public void UnityContainerAdapter_WithNullUnityContainer_ThrowsArgumentNullException()
        {
            // Act
            TestDelegate construct = () => new UnityContainerAdapter(null);

            // Assert
            Assert.That(construct, Throws.ArgumentNullException);
        }


        [Test]
        public void IsRegistered_WithType_CallsUnityContainer()
        {
            // Arrange
            var adapter = new UnityContainerAdapter(unityContainer);

            // Act
            adapter.IsRegistered<Service>();

            // Assert
            mockUnityContainer
                .Verify(
                    m => m.IsRegistered(
                        It.IsIn(typeof(Service)), 
                        It.IsAny<string>()), 
                    Times.Once);
        }


        [Test]
        public void Register_WithType_WithNotSingleton_CallsUnityContainer()
        {
            // Arrange
            var adapter = new UnityContainerAdapter(unityContainer);

            // Act
            adapter.Register<Service>(false);

            // Assert
            mockUnityContainer
                .Verify(
                    m => m.RegisterType(
                        It.IsAny<Type>(),
                        It.IsAny<Type>(),
                        It.IsAny<string>(),
                        It.IsAny<ITypeLifetimeManager>()),
                    Times.Once);
        }


        [Test]
        public void Register_WithType_WithSingleton_CallsUnityContainer()
        {
            // Arrange
            var adapter = new UnityContainerAdapter(unityContainer);

            // Act
            adapter.Register<Service>(true);

            // Assert
            mockUnityContainer
                .Verify(
                    m => m.RegisterType(
                        It.IsAny<Type>(),
                        It.IsAny<Type>(),
                        It.IsAny<string>(),
                        It.IsAny<ITypeLifetimeManager>()),
                    Times.Once);
        }


        [Test]
        public void Register_WithInstance_CallsUnityContainer()
        {
            // Arrange
            var adapter = new UnityContainerAdapter(unityContainer);
            var service = new Service();

            // Act
            adapter.Register(service);

            // Assert
            mockUnityContainer
                .Verify(
                    m => m.RegisterInstance(
                        It.IsIn(typeof(Service)), 
                        It.IsAny<string>(),
                        It.IsAny<object>(),
                        It.IsAny<IInstanceLifetimeManager>()),
                    Times.Once);
        }


        [Test]
        public void Register_WithSubType_WithNotSingleton_CallsUnityContainer()
        {
            // Arrange
            var adapter = new UnityContainerAdapter(unityContainer);

            // Act
            adapter.Register<Service, SubService>(false);

            // Assert
            mockUnityContainer
                .Verify(
                    m => m.RegisterType(
                        It.IsIn(typeof(Service)),
                        It.IsIn(typeof(SubService)),
                        It.IsAny<string>(),
                        It.IsAny<ITypeLifetimeManager>()),
                    Times.Once);
        }


        [Test]
        public void Register_WithSubType_WithSingleton_CallsUnityContainer()
        {
            // Arrange
            var adapter = new UnityContainerAdapter(unityContainer);

            // Act
            adapter.Register<Service, SubService>(true);

            // Assert
            mockUnityContainer
                .Verify(
                    m => m.RegisterType(
                        It.IsIn(typeof(Service)),
                        It.IsIn(typeof(SubService)),
                        It.IsAny<string>(),
                        It.IsAny<ITypeLifetimeManager>()),
                    Times.Once);
        }


        [Test]
        public void Resolve_WithType_CallsUnityContainer()
        {
            // Arrange
            var adapter = new UnityContainerAdapter(unityContainer);

            // Act
            var service = adapter.Resolve<Service>();

            // Assert
            mockUnityContainer
                .Verify(
                    m => m.Resolve(
                        It.IsIn(typeof(Service)),
                        It.IsAny<string>(),
                        It.IsAny<ResolverOverride[]>()), 
                    Times.Once);
        }
    }
}
