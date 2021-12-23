using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    class Furnace
    {
        /* 
         * L1, L2, L3 - линейные размеры камеры

         * t0 - температура окр среды
         * t1 - температура внутренней поверхности огнеупора
         * t2 - температура на границе раздела слоев
         * t3 - температура наружной поверхности корпуса печи
         * t4 - температура наружной поверхности футеровки
         * tz - температура наружной поверхности футеровки (для подбора)

         * y1 - коэффициент теплоотдачи (Вт/м2*К)
         * x1 - коэффициент теплопроводности огнеупора (Вт/м*К)
         * x2 - коэффициент теплопроводности теплоизоляции (Вт/м*К)
         * h1 - толщина огнеупора (м)
         * h2 - толщина теплоизоляции (м)

         * h3 - толщина огнеупора (для однослойной/двухслойной заглушки (дверцы)) (м)
         * h4 - толщина теплоизоляции (для двухслойной заглушки (дверцы)) (м)

         * q1 - плотность теплового потока (Вт/м2)
         */

        public enum DoorConstructionType
        {
            DOOR,
            PLUG
        }

        public enum DoorLayerType
        {
            SINGLE_LAYER,
            DOUBLE_LAYER
        }

        public enum FurnaceType
        {
            TUBE,
            CHAMBER
        }

        // Расчет футеровки камерной печи

        public static void CalculateInwall(double t1, double t3, double t0,
                                           double a1, double b1,
                                           double a2, double b2,
                                           double h1, double h3, double h4,
                                           double L1, double L2, double L3,
                                           DoorConstructionType constructionType,
                                           DoorLayerType layerType,
                                           out double t2, out double h2, out double Q1,
                                           
                                           out double F0, out double Ft,
                                           out double F1, out double F2, out double F3,
                                           out double y1, out double q1, out double L4)
        {
            double i = 9.304;
            double j = 0.05815;

            y1 = i + t3 * j;

            q1 = y1 * (t3 - t0);

            t2 = (-2 * a1 +
                        Math.Sqrt(4 * a1 * a1 - 4 * b1 * (2 * h1 * q1 - 2 * a1 * t1 - b1 * t1 * t1)))
                        / (2 * b1);
            double x1 = a1 + (b1 * (t1 + t2) * 0.5f);
            double x2 = a2 + (b2 * (t2 + t3) * 0.5f);

            h2 = x2 * (t2 - t3) / q1;

            // расчет Q

            L4 = 0.0;

            if (constructionType == DoorConstructionType.DOOR)
            {
                L4 = L3;
            }
            else if (constructionType == DoorConstructionType.PLUG)
            {
                if (layerType == DoorLayerType.SINGLE_LAYER)
                {
                    L4 = L3 - h3;
                }
                else if (layerType == DoorLayerType.DOUBLE_LAYER)
                {
                    L4 = L3 - h3 - h4;
                }
            }

            F1 = L1 * L2 + 2 * L1 * L4 + 2 * L2 * L4;
            F2 = (L1 + 2 * h1) * (L2 + 2 * h1) +
                 2 * (L1 + 2 * h1) * (L4 + h1) +
                 2 * (L2 + 2 * h1) * (L4 + h1);
            F3 = (L1 + 2 * h1 + 2 * h2) * (L2 + 2 * h1 + 2 * h2) +
                 2 * (L1 + 2 * h1 + 2 * h2) * (L4 + h1 + h2) +
                 2 * (L2 + 2 * h1 + 2 * h2) * (L4 + h1 + h2);

            F0 = 0;

            if (F2 / F1 <= 2)
            {
                F0 = (F1 + F2) / 2;
            }
            else if (F2 / F1 > 2)
            {
                F0 = Math.Sqrt(F1 * F2);
            }

            Ft = 0;

            if (F3 / F2 <= 2)
            {
                Ft = (F2 + F3) / 2;
            }
            else if (F3 / F2 > 2)
            {
                Ft = Math.Sqrt(F2 * F3);
            }

            Q1 = (t1 - t0) /
                        (h1 / (x1 * F0) + h2 / (x2 * Ft) + 1 / (y1 * F3));
        }

        // расчет однослойной дверцы (или заглушки)

        public static void CalculateSingleLayerDoor(double t1, double t0,
                                                    double a3, double b3,
                                                    double h3,
                                                    double L1, double L2,
                                                    FurnaceType furnaceType, double d0,
                                                    out double t4, out double Q2,
                                                    
                                                    out double q2, out double x3,
                                                    out double F4, out double y2)
        {
            double i = 9.304;
            double j = 0.05815;

            t4 = (1 / (2 * (b3 + 2 * h3 * j))) *
                        (
                            -(2 * a3 + 2 * h3 * i - 2 * h3 * j * t0) +
                            Math.Sqrt
                            (
                                (2 * a3 + 2 * h3 * i - 2 * h3 * j * t0) * (2 * a3 + 2 * h3 * i - 2 * h3 * j * t0) +
                                4 * (b3 + 2 * h3 * j) * (2 * a3 * t1 + b3 * t1 * t1 + 2 * h3 * i * t0)
                            )
                        );
            x3 = a3 + (b3 * (t1 + t4) * 0.5);
            y2 = i + j * t4;

            q2 = (t1 - t0) / (h3 / x3 + 1 / y2);

            F4 = 0;

            if (furnaceType == FurnaceType.CHAMBER)
            {
                F4 = L1 * L2;
            }
            else if (furnaceType == FurnaceType.TUBE)
            {
                F4 = Math.PI * SQ(d0) / 4;
            }

            Q2 = q2 * F4;
        }

        // расчет двухслойной заглушки (или дверцы)

        public static void CalculateDoubleLayerDoor(double t1, double t0,
                                                    double a3, double b3, double a4, double b4,
                                                    double h3, double h4,
                                                    double L1, double L2,
                                                    FurnaceType furnaceType, double d0,
                                                    out double t5, out double tz, out double Q2,
                                                    out double q2, out double q2Check,
                                                    
                                                    out double x3, out double x4,
                                                    out double F4, out double y2)
        {
            double i = 9.304;
            double j = 0.05815;
            tz = 0;
            double t4 = t0;
            t5 = 0;
            q2 = 0;
            x3 = 0;

            while(Math.Abs(t4 - tz) > 0.1)
            {
                t5 = (1 / (2 * (b3 * h4 + b4 * h3))) *
                        (
                            -(2 * a3 * h4 + 2 * a4 * h3) +
                                Math.Sqrt // в лабнике двойной корень, похоже опечатка
                                (
                                    (2 * a3 * h4 + 2 * a4 * h3) * (2 * a3 * h4 + 2 * a4 * h3) +
                                    4 * (b3 * h4 + b4 * h3) * (2 * a3 * h4 * t1 + b3 * h4 * t1 * t1 + 2 * a4 * h3 * t4 + b4*h3*t4*t4)
                                )
                        );
                x3 = a3 + (b3 * (t1 + t5) * 0.5);
                q2 = x3 * (t1 - t5) / h3;

                tz = (
                            -(i - j * t0) +
                            Math.Sqrt
                            (
                                (i - j * t0) * (i - j * t0) +
                                4 * j * (i * t0 + q2)
                            )
                        )
                        / (2 * j);

                t4 += 0.01;
            }

            x4 = a4 + b4 * (t5 + t4) * 0.5;
            y2 = i + j * t4;
            q2Check = x4 * (t5 - t4) / h4;


            F4 = 0;
            if (furnaceType == FurnaceType.CHAMBER)
            {
                F4 = L1 * L2;
            }
            else if(furnaceType == FurnaceType.TUBE)
            {
                F4 = Math.PI * SQ(d0) / 4;
            }

            Q2 = q2 * F4;
        }

        // расчет суммарного теплового потока

        public enum FurnaceHeaterMaterial
        {
            METAL,
            CARBORUNDUM,
            MOLYBDENUM_DISILICIDE
        }

        public static void CalculateTotalHeatFlux(double Q1, double Q2, FurnaceHeaterMaterial heaterMaterial,
                                           out double Q, out double P)
        {
            double Q3 = 0;

            switch (heaterMaterial)
            {
                case FurnaceHeaterMaterial.METAL:
                    Q3 = Q1 + Q2;
                    break;
                case FurnaceHeaterMaterial.CARBORUNDUM:
                    Q3 = 2 * (Q1 + Q2);
                    break;
                case FurnaceHeaterMaterial.MOLYBDENUM_DISILICIDE:
                    Q3 = 3 * (Q1 + Q2);
                    break;
            }

            Q = Q1 + Q2 + Q3;
            P = 0.001 * Q;
        }


        //расчет трубчатой печи
        public static void CalculateTubeFurnace(double L, double d0, double d2,
                                                double t0, double t1, double t3,
                                                double h1, double h3, double h4,
                                                double a1, double a2, double b1, double b2,
                                                DoorConstructionType constructionType,
                                                DoorLayerType layerType,
                                                out double t2, out double dz, out double q1, out double Q1,
                                                out double q1Check,
                                                
                                                out double x1, out double x2,
                                                out double d1, out double y1)
        {
            double i = 9.304;
            double j = 0.05815;
            
            d1 = d0 + 2 * h1;
            d2 = d1 + 0.001f;

            x1 = 0;
            dz = 0;
            t2 = 0;
            q1 = 0;

            while (Math.Abs(dz - d2) > 0.001f)
            {
                t2 = (1 / (2 * (b1 * LN(d2 / d1) + b2 * LN(d1 / d0)))) *
                        (
                            (
                                -(2 * a1 * LN(d2 / d1) + 2 * a2 * LN(d1 / d0))
                            ) //в выражении есть некое "d" без индекса, видимо опечатка
                            +
                            Math.Sqrt
                            (
                                SQ
                                (
                                    2 * a1 * LN(d2 / d1) + 2 * a2 * LN(d1 / d0)
                                )
                                +
                                4 * (b1 * LN(d2 / d1) + b2 * LN(d1 / d0)) //множитель "4" в книжке пропущен и порядок скобок неверный
                                *
                                (
                                    (2 * a1 * t1 + b1 * t1 * t1) * LN(d2 / d1)
                                    +
                                    (2 * a2 * t3 + b2 * t3 * t3) * LN(d1 / d0)
                                )
                            )
                        );
                x1 = a1 + b1 * (t1 + t2) / 2;
                q1 = (2 * Math.PI * x1 * (t1 - t2)) / LN(d1 / d0);

                dz = q1 / (Math.PI * (i + j * t3) * (t3 - t0));

                d2 += 0.0001f;
            }

            x2 = a2 + b2 * (t2 + t3) / 2;
            y1 = i + j * t3;

            q1Check = Math.PI * (t1 - t0)/
                             (
                                1/((2*x1)*LN(d1/d0)) + 
                                1/((2*x2)*LN(d2/d1)) + 
                                1/(y1*d2)
                             );

            double L4 = 0;

            if (constructionType == DoorConstructionType.DOOR)
            {
                L4 = L;
            }
            else
            {
                if (layerType == DoorLayerType.SINGLE_LAYER)
                {
                    L4 = L - 2 * h3;
                }
                else if (layerType == DoorLayerType.DOUBLE_LAYER)
                {
                    L4 = L - 2 * h3 - 2 * h4;
                }
            }

            Q1 = q1 * L4;
        }

        /*
         * Tn - температура нагревателя (К)
         * Tm - температура нагреваемого материала (К)
         * Wi - удельная поверхностная мощность идеального нагревателя (кВт/м2)
         * J - коэффициент эффективности излучения
         * i, j - коэффициенты для расчета удельного сопротивления
         * 
         */

        public enum ConnectionType
        {
            LINEAR,
            SINGLE_PHASE,
            TRIANGLE,
            STAR
        }

        public enum HeaterForm
        {
            WIRE,
            PLATE
        }

        public enum HeaterArrangement
        {
            LINEAR,
            ZIGZAG,
            SPIRAL
        }

        public enum HeaterPlacement
        {
            OPEN,
            OPEN_ON_TUBE,
            ON_SHELF,
            IN_SLOT
        }

        public static double[] recommendedWireD = new double[]
        {
            0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0,
            1.1, 1.2, 1.4, 1.6, 1.8, 2.0, 2.2, 2.5, 2.8, 3.0
        };

        public static double[] recommendedPlateA = new double[]
        {
            0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0,
            1.1, 1.2, 1.4, 1.6, 1.8, 2.0, 2.2, 2.5, 2.8, 3.0
        };

        public static double[] recommendedPlateB = new double[]
        {
            1, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 25, 30
        };

        public struct HeaterMaterial
        {
            public string name;
            public double i;
            public double j;
            public double tMax;

            public HeaterMaterial(string name, double tMax, double i, double j)
            {
                this.name = name;
                this.i = i;
                this.j = j;
                this.tMax = tMax;
            }
        }

        public static HeaterMaterial[] heaterMaterials = new HeaterMaterial[]
        {
            new HeaterMaterial("Фехраль Х1ЗЮ4",        700,  1.26*E(-6),  0.6* E(-10)),
            new HeaterMaterial("Сталь Х25Н20С2",       800,  0.92*E(-6),  3.8* E(-10)),
            new HeaterMaterial("Нихром Х15Н60",        950,  1.1* E(-6),  1.4* E(-10)),
            new HeaterMaterial("Нихром Х20Н80",       1100,  1.1* E(-6),  8.5* E(-11)),
            new HeaterMaterial("Сплав ОХ23Ю5А",       1150,  1.4* E(-6),  0.5* E(-10)),
            new HeaterMaterial("Сплав ОХ27Ю5А",       1250,  1.4* E(-6),  0.5* E(-10)),
            new HeaterMaterial("Карборунд",           1450,  0.8* E(-3),  1.9* E(-3)),
            new HeaterMaterial("Дисилицид молибдена", 1650,  3.6* E(-6),  0)
        };

        public static void CalculateMetalHeater(double Tn, double Tm, string matName,
                                                double ULinear, double P, double m,
                                                ConnectionType connectionType, HeaterForm heaterForm, HeaterArrangement arrangement, HeaterPlacement placement,
                                                out double Uf, out double Pf, out double If, out double Ilinear, out double R,
                                                out double LAccurate, out double LLinear, out double wireDRecommended, out double W,
                                                out double Wi, out string matDebugName, out double plateBRecommended, out double plateARecommended)
        {
            HeaterMaterial material = Array.Find(heaterMaterials, mat => mat.name.Equals(matName));

            Tn += 273;
            Tm += 273;

            matDebugName = matName + "; " + material.name;
            double i = material.i;
            double j = material.j;

            double J = 0;

            //Получаем значение J
            switch (heaterForm)
            {
                case HeaterForm.WIRE:

                    switch (arrangement)
                    {
                        case HeaterArrangement.LINEAR:

                            switch (placement)
                            {
                                case HeaterPlacement.OPEN:
                                    J = 0.39;
                                    break;
                                case HeaterPlacement.OPEN_ON_TUBE:
                                    J = -1; //не существует
                                    break;
                                case HeaterPlacement.ON_SHELF:
                                    J = 0.37;
                                    break;
                                case HeaterPlacement.IN_SLOT:
                                    J = 0.34;
                                    break;
                            }

                            break;
                        case HeaterArrangement.ZIGZAG:

                            switch (placement)
                            {
                                case HeaterPlacement.OPEN:
                                    J = 0.55;
                                    break;
                                case HeaterPlacement.OPEN_ON_TUBE:
                                    J = -1; //не существует
                                    break;
                                case HeaterPlacement.ON_SHELF:
                                    J = 0.54;
                                    break;
                                case HeaterPlacement.IN_SLOT:
                                    J = 0.535;
                                    break;
                            }

                            break;
                        case HeaterArrangement.SPIRAL:

                            switch (placement)
                            {
                                case HeaterPlacement.OPEN:
                                    J = 0.555;
                                    break;
                                case HeaterPlacement.OPEN_ON_TUBE:
                                    J = 0.55;
                                    break;
                                case HeaterPlacement.ON_SHELF:
                                    J = 0.545;
                                    break;
                                case HeaterPlacement.IN_SLOT:
                                    J = 0.54;
                                    break;
                            }

                            break;
                    }

                    break;

                case HeaterForm.PLATE:

                    switch (arrangement)
                    {
                        case HeaterArrangement.LINEAR:

                            switch (placement)
                            {
                                case HeaterPlacement.OPEN:
                                    J = 0.5;
                                    break;
                                case HeaterPlacement.OPEN_ON_TUBE:
                                    J = -1; //не существует
                                    break;
                                case HeaterPlacement.ON_SHELF:
                                    J = 0.45;
                                    break;
                                case HeaterPlacement.IN_SLOT:
                                    J = 0.35;
                                    break;
                            }

                            break;
                        case HeaterArrangement.ZIGZAG:

                            switch (placement)
                            {
                                case HeaterPlacement.OPEN:
                                    J = 0.505;
                                    break;
                                case HeaterPlacement.OPEN_ON_TUBE:
                                    J = -1; //не существует
                                    break;
                                case HeaterPlacement.ON_SHELF:
                                    J = 0.47;
                                    break;
                                case HeaterPlacement.IN_SLOT:
                                    J = 0.355; //в книге 0.535, но в программе явно 0.355
                                    break;
                            }

                            break;
                        case HeaterArrangement.SPIRAL:
                            //не существует
                            J = -1;
                            break;
                    }

                    break;
            }

            Wi = 5.7 * Math.Pow(10, -11) * (Math.Pow(Tn, 4) - Math.Pow(Tm, 4));
            W = Wi * J;

            double ro = i + j * (Tn-273);
            Uf = 0;
            If = 0;
            Pf = 0;

            //get Uf Pf
            switch (connectionType)
            {
                case ConnectionType.LINEAR:
                    {
                        Uf = ULinear;
                        Pf = P;

                        break;
                    }
                case ConnectionType.SINGLE_PHASE:
                    {
                        Uf = ULinear / Math.Sqrt(3);
                        Pf = P;

                        break;
                    }
                case ConnectionType.TRIANGLE:
                    {
                        Uf = ULinear;
                        Pf = P / 3;

                        break;
                    }
                case ConnectionType.STAR:
                    {
                        Uf = ULinear / Math.Sqrt(3);
                        Pf = P / 3;

                        break;
                    }
            }

            double L = 0;
            double d = 0; //диаметр проволоки или толщина пластины
            double S = 0;

            if (heaterForm == HeaterForm.WIRE)
            {
                L = 0.043 * Math.Pow(Uf * Uf * Pf / ro / W / W, 1.0 / 3.0);
                d = 7.4 * Math.Pow(ro * Pf * Pf / Uf / Uf / W, 1.0 / 3.0);
                S = Math.PI * d * d / 4;
            }
            else if (heaterForm == HeaterForm.PLATE)
            {
                L = 0.063 * Math.Pow(m * Uf * Uf * Pf / ((m + 1) * (m + 1) * ro * W * W), 1.0 / 3.0);
                d = 7.937 * Math.Pow(ro * Pf * Pf / (m * (m + 1) * Uf * Uf * W), 1.0 / 3.0);
                S = d * m * d;
            }

            LAccurate = 0;

            wireDRecommended = GetCeilingFromArray(recommendedWireD, d * 1000) / 1000;
            plateARecommended = GetCeilingFromArray(recommendedPlateA, d * 1000) / 1000;
            plateBRecommended = GetCeilingFromArray(recommendedPlateB, d * m * 1000) / 1000;

            LLinear = 0;
            double t;
            double D;
            double H;

            R = 0.975578 * ro * L / S;

            switch (heaterForm)
            {
                case HeaterForm.WIRE:

                    LAccurate = Math.PI * wireDRecommended * wireDRecommended * R / (4 * ro);

                    switch (arrangement)
                    {
                        case HeaterArrangement.LINEAR:
                            LLinear = LAccurate;
                            break;
                        case HeaterArrangement.ZIGZAG:
                            t = 5.5 * wireDRecommended;
                            H = 10 * wireDRecommended;
                            LLinear = t * LAccurate / (t + 2 * H);
                            break;
                        case HeaterArrangement.SPIRAL:
                            t = 2 * wireDRecommended;
                            D = 10 * wireDRecommended;
                            LLinear = t * LAccurate / (Math.PI * D);
                            break;
                    }
                    break;

                case HeaterForm.PLATE:

                    LAccurate = plateARecommended * plateBRecommended * R / ro;

                    switch (arrangement)
                    {
                        case HeaterArrangement.LINEAR:
                            LLinear = LAccurate;
                            break;
                        case HeaterArrangement.ZIGZAG:
                            t = 1.8 * (plateBRecommended);
                            H = 10 * (plateARecommended);
                            LLinear = t * LAccurate / (t + 2 * H);
                            break;
                        case HeaterArrangement.SPIRAL:
                            //не существует
                            break;
                    }
                    break;
            }

            If = Uf / R;
            Ilinear = 0;

            switch (connectionType)
            {
                case ConnectionType.LINEAR:
                    Ilinear = If;
                    break;
                case ConnectionType.SINGLE_PHASE:
                    Ilinear = If;
                    break;
                case ConnectionType.TRIANGLE:
                    Ilinear = If * Math.Sqrt(3);
                    break;
                case ConnectionType.STAR:
                    Ilinear = If;
                    break;
            }
        }


        //расчет карборундного нагревателя
        public struct CarborundHeater
        {
            public string name;
            public double Lp;
            public double L;
            public double d;
            public double D;
            public double f;
            public double R;

            public CarborundHeater(string name, double Lp, double L, double d, double D, double f, double R)
            {
                this.name = name;
                this.Lp = Lp;
                this.L = L;
                this.d = d;
                this.D = D;
                this.f = f;
                this.R = R;
            }
        }

        public static CarborundHeater[] carborundHeaters = new CarborundHeater[]
        {
            new CarborundHeater("КНМ-8х100х270", 0.10, 0.27, 0.008, 0.014, 2.51, 2.0),
            new CarborundHeater("КНМ-8х150х270", 0.15, 0.27, 0.008, 0.014, 3.77, 3.0),
            new CarborundHeater("КНМ-8х150х320", 0.15, 0.32, 0.008, 0.014, 3.77, 3.0),
            new CarborundHeater("КНМ-8х150х450", 0.15, 0.42, 0.008, 0.014, 3.77, 3.0),
            new CarborundHeater("КНМ-8х180х300", 0.18, 0.30, 0.008, 0.014, 4.52, 3.6),
            new CarborundHeater("КНМ-8х180х350", 0.18, 0.35, 0.008, 0.014, 4.52, 3.6),
            new CarborundHeater("КНМ-8х180х400", 0.18, 0.40, 0.008, 0.014, 4.52, 3.6),
            new CarborundHeater("КНМ-8х180х480", 0.18, 0.48, 0.008, 0.014, 4.52, 3.6),
            new CarborundHeater("КНМ-8х200х500", 0.20, 0.50, 0.008, 0.014, 5.03, 4.0),
            new CarborundHeater("КНМ-8х250х450", 0.25, 0.45, 0.008, 0.014, 6.28, 5.0),
            new CarborundHeater("КНМ-12х250х750", 0.25, 0.75, 0.012, 0.018, 9.42, 3.0),
            new CarborundHeater("КНМ-14х300х250", 0.30, 0.80, 0.014, 0.023, 13.20, 3.5)

        };

        public static void CalculateCarborundHeater(double Tn, double Tm, string matName,
                                                    double J, double P,
                                                    
                                                    out double single_I_sequence, out double single_U_sequence,
                                                    out double single_I_parallel, out double single_U_parallel,

                                                    out double triangle_I_sequence, out double triangle_U_sequence,
                                                    out double triangle_I_parallel, out double triangle_U_parallel,

                                                    out double star_I_sequence, out double star_U_sequence,
                                                    out double star_I_parallel, out double star_U_parallel,
                                                    
                                                    out double U, out double Up, out double I, out double N,
                                                    out double n, out double P1, out double W, out double Wi)
        {
            CarborundHeater type = Array.Find(carborundHeaters, mat => mat.name.Equals(matName));

            Tn += 273;
            Tm += 273;

            Wi = 5.7 * Math.Pow(10, -11) * (Math.Pow(Tn, 4) - Math.Pow(Tm, 4));
            W = Wi * J;

            P1 = type.f * 0.001 * W;
            N = (int) (P/P1);
            if (P % P1 > 0)
            {
                N++;
            }

            U = Math.Sqrt(1000 * P1 * type.R);
            Up = 3 * U;
            I = U / type.R;
            n = N / 3;

            single_I_sequence = I;
            single_U_sequence = Up * N;
            single_I_parallel = I * N;
            single_U_parallel = Up;

            triangle_I_sequence = I * Math.Sqrt(3);
            triangle_U_sequence = Up * n;
            triangle_I_parallel = I * n * Math.Sqrt(3);
            triangle_U_parallel = Up;

            star_I_sequence = I;
            star_U_sequence = Up * n * Math.Sqrt(3);
            star_I_parallel = I * n;
            star_U_parallel = Up * Math.Sqrt(3);
        }


        //расчет ДМ нагревателя
        public static Dictionary<int, double> MoSi2Resistance = new Dictionary<int, double>
        {
            {700, 1.5f},
            {1400, 2.8f},
            {1450, 2.9f},
            {1500, 3.0f},
            {1550, 3.1f},
            {1600, 3.2f}
        };
        
        public struct MoSi2Heater
        {
            public string name;
            public double L;
            public double Lp;
            public double LB;
            public double f;

            public MoSi2Heater(string name, double l, double lp, double lB, double f)
            {
                this.name = name;
                L = l;
                Lp = lp;
                LB = lB;
                this.f = f;
            }
        }

        public static MoSi2Heater[] moSi2Heaters = new MoSi2Heater[]
        {
            new MoSi2Heater("ДМ-180x250", 0.18,  0.39, 0.25, 7.35),
            new MoSi2Heater("ДМ-180x400", 0.18,  0.39, 0.4,  7.35),
            new MoSi2Heater("ДМ-250x250", 0.25,  0.53, 0.25, 9.99),
            new MoSi2Heater("ДМ-250x400", 0.25,  0.53, 0.4,  9.99),
            new MoSi2Heater("ДМ-315x250", 0.315, 0.66, 0.25, 12.44),
            new MoSi2Heater("ДМ-315x400", 0.315, 0.66, 0.4,  12.44),
            new MoSi2Heater("ДМ-315x500", 0.315, 0.66, 0.5,  12.44),
            new MoSi2Heater("ДМ-400x250", 0.4,   0.83, 0.25, 15.64),
            new MoSi2Heater("ДМ-400x400", 0.4,   0.83, 0.4,  15.64),
            new MoSi2Heater("ДМ-400x500", 0.4,   0.83, 0.5,  15.64),
            new MoSi2Heater("ДМ-500x250", 0.5,   1.03, 0.25, 19.42),
            new MoSi2Heater("ДМ-500x400", 0.5,   1.03, 0.4,  19.42),
            new MoSi2Heater("ДМ-500x500", 0.5,   1.03, 0.5,  19.42),
            new MoSi2Heater("ДМ-630x250", 0.63,  1.29, 0.25, 24.32),
            new MoSi2Heater("ДМ-630x400", 0.63,  1.29, 0.4,  24.32),
            new MoSi2Heater("ДМ-630x500", 0.63,  1.29, 0.5,  24.32),
            new MoSi2Heater("ДМ-800x700", 0.80,  1.63, 0.7,  30.73)
        };

        public static void CalculateMoSi2Heater(double Tn, double Tm, string heaterName,
                                                double P,

                                                out double single_I_sequence, out double single_U_sequence,
                                                out double single_I_parallel, out double single_U_parallel,

                                                out double triangle_I_sequence, out double triangle_U_sequence,
                                                out double triangle_I_parallel, out double triangle_U_parallel,

                                                out double star_I_sequence, out double star_U_sequence,
                                                out double star_I_parallel, out double star_U_parallel,

                                                out double U, out double Up, out double I, out double N,
                                                out double n, out double P1, out double W, out double Wi)
        {
            MoSi2Heater heater = Array.Find(moSi2Heaters, h => h.name.Equals(heaterName));

            Tn += 273;
            Tm += 273;

            int Tc = (int)Tn - 273;

            double J = 0.87;

            //значения даны, но в расчетах не используются
            double dp = 0.006;
            double dB = 0.012;
            double tB = 0.03;

            Wi = 5.7 * Math.Pow(10, -11) * (Math.Pow(Tn, 4) - Math.Pow(Tm, 4));
            W = Wi * J;

            double Pp = heater.f * 0.001 * W;
            
            double ro;
            MoSi2Resistance.TryGetValue(Tc, out ro);
            if (ro == 0) ro = 0.001880597 * Tn - 0.3335522;

            P1 = Pp * (1 + (7.5 * E(-7) * heater.LB / (ro * E(-6) * heater.Lp)));

            N = (int) Math.Ceiling(P / P1);
            n = ((double)N) / 3;

            double R = 3.54 * E(4) * (ro * E(-6) * heater.Lp + 7.5 * E(-7) * heater.LB);
            U = Math.Sqrt(E(3) * P1 * R);
            I = U / R;
            Up = 3 * U;

            single_I_sequence = I;
            single_U_sequence = Up * N;
            single_I_parallel = I * N;
            single_U_parallel = Up;
            
            triangle_I_sequence = I * Math.Sqrt(3);
            triangle_U_sequence = Up * n;
            triangle_I_parallel = I * n * Math.Sqrt(3);
            triangle_U_parallel = Up;
            
            star_I_sequence = I;
            star_U_sequence = Up * n * Math.Sqrt(3);
            star_I_parallel = I * n;
            star_U_parallel = Up * Math.Sqrt(3);
        }

        public static double LN(double value)
        {
            return Math.Log(value);
        }

        public static double SQ(double value)
        {
            return value * value;
        }

        public static double Lerp(double firstFloat, double secondFloat, double by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }

        public static double GetCeilingFromArray(double[] array, double value)
        {
            double closestValue = array[0];
            for (int i = 0; i < array.Length; i++)
            {
                if (value - array[i] < 0)
                {
                    return array[i];
                }
            }
            return closestValue;
        }

        public static double E(int power)
        {
            return Math.Pow(10, power);
        }
    }
}
