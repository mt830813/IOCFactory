using System;
using IOCFactory;
using IOCFactoryModel;
using IOCFactory.Model.Imp.InstCreator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Configuration;

namespace IOCFactoryUnitTest
{

    public interface Animal
    {
        string Hawl();
    }

    public interface Toy
    {
        string Play();
    }

    public class Ball : Toy
    {

        public string Play()
        {
            return "roll and roll";
        }
    }


    public class Dog : Animal
    {
        public string Hawl()
        {
            var returnValue = "wang wang wang";
            return returnValue;
        }
    }

    public class Cat : Animal
    {
        public string Hawl()
        {
            var returnValue = "miao miao miao";
            return returnValue;
        }
    }

    public class CatDog : Animal
    {
        private Animal toDecorate;
        public CatDog(Animal toDecorate)
        {
            this.toDecorate = toDecorate;
        }

        public string Hawl()
        {
            var returnValue = toDecorate.Hawl();
            returnValue += " wang wang wang";
            return returnValue;
        }
    }

    public class MachineCatDog : Animal
    {
        private Animal toDecorate;
        public MachineCatDog(Animal toDecorate)
        {
            this.toDecorate = toDecorate;
        }

        public string Hawl()
        {
            var returnValue = toDecorate.Hawl();
            returnValue += " hmm hmm hmm";
            return returnValue;
        }
    }

    public class SingleTonTest : Animal
    {
        private SingleTonTest() { }

        public string Hawl()
        {
            return this.GetHashCode().ToString();
        }
    }

    public class DITest
    {
        public Animal animal;
        public Toy toy;
        public DITest(Animal animal, Toy toy)
        {
            this.animal = animal;
            this.toy = toy;
        }

        public string Test()
        {
            return animal.Hawl() + this.toy.Play();
        }
    }

    [TestClass]
    public class IOCFactoryTest
    {
        [TestInitialize()]
        public void Init()
        {
            Factory factory = Factory.GetInst();
            factory.Clear();
            factory.Regist<Animal, Cat>("cat", InstType.Normal);
            factory.Regist<Animal, Dog>("dog", InstType.Normal);
            factory.Regist<Animal, SingleTonTest>(InstType.Singleton);
            factory.RegistDecorate<Animal, CatDog>("catDog", "cat");
            factory.RegistDecorate<Animal, MachineCatDog>("catDog", "cat");

            factory.Regist<Toy, Ball>(InstType.Normal);

            factory.Regist<DITest, DITest>(InstType.DISingleton);

            factory.Regist<DITest, DITest>("Test", InstType.DI);


        }
        [TestCleanup]
        public void CleanUp()
        {

        }

        [TestMethod]
        public void DIInstTest2()
        {
            Factory factory = Factory.GetInst();
            var a = factory.Get<DITest>("Test");
            var b = factory.Get<DITest>("Test");
            Assert.AreEqual(a.animal.GetHashCode(), b.animal.GetHashCode());
            Assert.AreNotEqual(a.toy.GetHashCode(), b.toy.GetHashCode());
        }



        [TestMethod]
        public void NormalInstTest()
        {
            Factory factory = Factory.GetInst();
            var result = new Dog();
            var dog = factory.Get<Animal>("dog");
            Assert.AreEqual(dog.Hawl(), result.Hawl());
            var cat = factory.Get<Animal>("cat");
            Assert.AreNotEqual(cat.Hawl(), result.Hawl());
        }

        [TestMethod]
        public void SingleInstTest()
        {
            Factory factory = Factory.GetInst();
            var obj1 = factory.Get<Animal>();
            var obj2 = factory.Get<Animal>();
            Assert.AreEqual(obj1, obj2);
        }

        [TestMethod]
        public void DecorateInstTest()
        {
            Factory factory = Factory.GetInst();
            var dog = factory.Get<Animal>("dog");
            var cat = factory.Get<Animal>("cat");
            var catDog = factory.Get<Animal>("catDog");
            Assert.AreEqual(cat.Hawl() + " " + dog.Hawl() + " hmm hmm hmm", catDog.Hawl());
        }

        [TestMethod]
        public void DIInstTest()
        {
            Factory factory = Factory.GetInst();

            var ani = factory.Get<Animal>();
            var toy = factory.Get<Toy>();

            var result = factory.Get<DITest>();
            var result2 = factory.Get<DITest>();
            var exp = new DITest(ani, toy);

            Assert.AreEqual(exp.Test(), result.Test());
            Assert.AreEqual(result, result2);
        }

        [TestMethod]
        public void MultiThreadTest()
        {
            try
            {
                int count = 100;

                Action func = () => { for (var i = 0; i < count; i++) { DIInstTest(); } };

                var a = new Thread(new ThreadStart(func));
                var b = new Thread(new ThreadStart(func));
                var c = new Thread(new ThreadStart(func));

                a.Start();
                b.Start();
                c.Start();
            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void RegistCustomerMethodTest()
        {
            var factory = Factory.GetInst();
            factory.RegistCustomerGetFunc(p => p == typeof(Animal), q => new Dog(), IOCFactoryModel.Enum.CustomerMethodEffectEnum.Once);

            var exp = new Dog();
            var inst = factory.Get<Animal>();

            Assert.AreEqual(exp.Hawl(), inst.Hawl());
        }

        [TestMethod]
        [DeploymentItem("inst.json")]
        public void RegistFromFileTest()
        {
            try
            {
                Factory factory = Factory.GetInst();
                factory.RegistFromFile("inst.json");

                Animal obj = factory.Get<Animal>("FileTest");
                Animal obj2 = factory.Get<Animal>("FileTest");

                Assert.AreEqual(obj, obj2);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        [DeploymentItem("setting.xml")]
        public void RegistFormUnityPatternSettingFileTest()
        {
            try
            {
                var factory = Factory.GetInst();

                factory.Clear();

                factory.RegistFromFile("setting.xml", IOCFactoryModel.Enum.FactoryMappingFilePattern.Unity);

                Animal obj = factory.Get<Animal>("FileTest");
                Animal obj2 = factory.Get<Animal>("FileTest");

                Assert.AreEqual(obj, obj2);

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        [DeploymentItem("setting.xml")]
        public void RegistFromSection()
        {
            var factory = Factory.GetInst();
            factory.Clear();
            factory.RegistFromSection("unity");
            Animal obj = factory.Get<Animal>("FileTest");
            Animal obj2 = factory.Get<Animal>("FileTest");

            Assert.AreEqual(obj, obj2);
        }

    }
}
