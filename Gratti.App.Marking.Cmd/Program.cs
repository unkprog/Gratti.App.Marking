using System;
using System.Collections.Generic;
using System.Reflection;
using Gratti.App.Marking.Api.Model;
using Gratti.App.Marking.Core.Extensions;
using Gratti.App.Marking.Core.Interfaces;
using Gratti.App.Marking.Model;
using Gratti.App.Marking.Utils;

namespace Gratti.App.Marking.Cmd
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Коммандная строка Gratti.App.Marking.Cmd");
            Console.WriteLine("Версия: " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("");
            if (args == null)
            {
                logger.Log("Не заданы параметры коммандной строки!");
                Console.Beep();
                Console.ReadKey();
                return;
            }

            appState = GetAppState();

            if (appState != null)
            {

                logger.Log("Чтение параметров...");
                foreach (string arg in args)
                {
                    string[] parse = arg.Split('=');
                    if (parse.Length > 0)
                    {
                        if (parse[0] == "-orders")
                            GenerateOrders((parse.Length > 1 ? parse[1] : string.Empty));
                    }
                }
                logger.Log("Завершено...");
            }
            else
            {
                logger.Log("НЕ ЗАДАНЫ НАСТРОЙКИ ПОДКЛЮЧЕНИЯ...");
                Console.Beep();
                Console.ReadKey();
            }
        }

        class MainLogger : ILoggerOutput
        {
            public void Log(string logRecord)
            {
                Console.WriteLine(string.Concat(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), ": ", logRecord));
            }
        }

        private static ILoggerOutput logger = new MainLogger();
        private static AppState appState;
        private static AppState GetAppState()
        {
            AppState result = null;
            SettingModel setting = IO.GetSetting();
            ProfileInfoModel profile = IO.GetCurrentProfile(setting);
            string msg = IO.VerifytProfile(profile);
            if (string.IsNullOrEmpty(msg))
            {
                result = new AppState();
                result.SetProfile(IO.GetCurrentProfile(IO.GetSetting()), logger);
            }
            else
                logger?.Log(msg);

            return result;
        }

        private static void GenerateOrders(string data)
        {
            List<OrderNewModel> orders = new List<OrderNewModel>();
            string[] parse = data.Split('|');

            logger.Log("Формирование списка ордеров...");
            foreach (string item in parse)
            {
                OrderNewModel order = CreateOrder(item);
                if (order != null)
                    orders.Add(order);
            }

            appState.Auth.Connect();
            logger.Log("Создание ордеров...");
            for (int i = 0, iCount = orders.Count; i < iCount; i++)
            {
                OrderNewModel order = orders[i];
                OrderNewProductModel product = order.Products[0];
                if (!string.IsNullOrEmpty(product.Gtin) && product.Quantity > 0)
                {
                    logger.Log("Создание ордера " + i.ToString() + " из " + iCount.ToString() + " (GTIN:" + product.Gtin + ", Количество:" + product.Quantity.ToString() + ")...");
                    string signContent = Utils.Certificate.SignByCertificateDetached(appState.Auth.GetCertificate(), order);
                    try
                    {
                        OrderResultModel orderResult = appState.OmsApi.PostOrder(appState.Auth.OmsToken, Api.GroupEnum.lp, order, signContent);
                        if (orderResult != null)
                            logger.Log("Ордер " + i.ToString() + " из " + iCount.ToString() + " создан: " + orderResult.OrderId + "...");
                    }
                    catch(Exception ex)
                    {
                        logger.Log("Ордер " + i.ToString() + " из " + iCount.ToString() + " - ОШИБКА! " + Environment.NewLine + ex.GetMessages());
                    }
                }
                else
                    logger.Log("Некорректные данные ордера ордера (GTIN:" + product.Gtin + ", Количество:" + product.Quantity.ToString() + ")...");
            }



        }

        private static OrderNewModel CreateOrder(string item)
        {
            if (string.IsNullOrEmpty(item))
                return null;

            string[] values = item.Split(';');
            int q;

            OrderNewModel order = new OrderNewModel()
            {
                CreateMethodType = OrderNewModel.CreateMethodTypeEnum.SELF_MADE,
                ReleaseMethodType = OrderNewModel.ReleaseMethodTypeEnum.PRODUCTION
            };
            OrderNewProductModel product = new OrderNewProductModel()
            {
                Quantity = 0,
                CisType = OrderNewProductModel.CisTypeEnum.UNIT,
                SerialNumberType = OrderNewProductModel.SerialNumerTypeEnum.OPERATOR,
                TemplateId = 10
            };
            order.Products.Add(product);

            if (values.Length > 0)
                order.ProductionOrderId = values[0];

            if (values.Length > 1)
                product.Gtin = values[1];
           
            if (values.Length > 2 && int.TryParse(values[2], out q))
                product.Quantity = q;

            if (values.Length > 3 && int.TryParse(values[3], out q))
                order.ReleaseMethodType = (OrderNewModel.ReleaseMethodTypeEnum)q;

            return order;
        }
    }
}
