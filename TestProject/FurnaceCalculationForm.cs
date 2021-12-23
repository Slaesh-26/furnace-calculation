using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestProject
{
    public partial class FurnaceCalculationForm : Form
    {
        public FurnaceCalculationForm()
        {
            InitializeComponent();
        }

        //рассчитать футеровку камерной печи (выполняется по нажатию кнопки "Рассчитать")
        private void inwall_calculateButtonCLick(object sender, EventArgs e)
        {
            //считываем введенные данные
            double t1 = (double)t1Value.Value;
            double t0 = (double)t0Value.Value;
            double t3 = (double)t3Value.Value;

            double a1 = (double)a1Value.Value;
            double b1 = (double)b1Value.Value;
            double a2 = (double)a2Value.Value;
            double b2 = (double)b2Value.Value;

            double h1 = (double)h1Value.Value;
            double h3 = (double)cf_h3Value.Value;
            double h4 = (double)cf_h4Value.Value;

            double L1 = (double)L1Value.Value;
            double L2 = (double)L2Value.Value;
            double L3 = (double)L3Value.Value;

            Furnace.DoorConstructionType doorConstructionType = Furnace.DoorConstructionType.DOOR;

            if (this.doorConstructionType.Text.Equals("Дверца"))
            {
                doorConstructionType = Furnace.DoorConstructionType.DOOR;
            }
            else if (this.doorConstructionType.Text.Equals("Заглушка"))
            {
                doorConstructionType = Furnace.DoorConstructionType.PLUG;
            }
            else
            {
                this.doorConstructionType.Text = "Дверца";
                doorConstructionType = Furnace.DoorConstructionType.DOOR;
            }

            Furnace.DoorLayerType doorLayerType = Furnace.DoorLayerType.SINGLE_LAYER;

            if (this.doorLayerType.Text.Equals("Однослойная"))
            {
                doorLayerType = Furnace.DoorLayerType.SINGLE_LAYER;
            }
            else if (this.doorLayerType.Text.Equals("Двухслойная"))
            {
                doorLayerType = Furnace.DoorLayerType.DOUBLE_LAYER;
            }
            else
            {
                this.doorLayerType.Text = "Однослойная";
                doorLayerType = Furnace.DoorLayerType.SINGLE_LAYER;
            }

            double t2;
            double h2;
            double Q1;

            double Ft, F0, F1, F2, F3, y1, q1, L4;

            Furnace.CalculateInwall(t1, t3, t0, a1, b1, a2, b2, h1, h3, h4, L1, L2, L3, doorConstructionType, doorLayerType,
                                    out t2, out h2, out Q1,
                                    out F0, out Ft, out F1, out F2, out F3,
                                    out y1, out q1, out L4);

            //выводим результаты

            t2Result.Text = GetRoundString(t2, 3);
            h2Result.Text = GetRoundString(h2, 3);
            Q1Result.Text = GetRoundString(Q1, 3);

            cf_FtResult.Text = GetRoundString(Ft, 3);
            cf_F0Result.Text = GetRoundString(F0, 3);
            cf_F1Result.Text = GetRoundString(F1, 3);
            cf_F2Result.Text = GetRoundString(F2, 3);
            cf_F3Result.Text = GetRoundString(F3, 3);
            cf_y1Result.Text = GetRoundString(y1, 3);
            cf_L4Result.Text = GetRoundString(L4, 3);
            cf_q1SmallResult.Text = GetRoundString(q1, 3);

            try 
            {
                //присваиваем значение Q1 для вкладки суммарного теплового потока, чтобы не переносить значение вручную
                //при некорректных исходных данных получим ошибку, поэтому лучше обернуть в try-catch
                thf_Q1Value.Value = (decimal)Q1; 
            }
            catch (Exception){}
        }

        private void sld_calculateButtonClick(object sender, EventArgs e)
        {
            double t0 = (double)sld_t0Value.Value;
            double t1 = (double)sld_t1Value.Value;
            double a3 = (double)sld_a3Value.Value;
            double b3 = (double)sld_b3Value.Value;
            double h3 = (double)sld_h3Value.Value;
            double L1 = (double)sld_L1Value.Value;
            double L2 = (double)sld_L2Value.Value;
            double d0 = (double)sld_d0Value.Value;

            double t4;
            double Q2;

            double x3, F4, y2, q2;

            Furnace.FurnaceType furnaceType = Furnace.FurnaceType.CHAMBER;

            if (sld_FurnaceType.Text == "Камерная")
            {
                furnaceType = Furnace.FurnaceType.CHAMBER;
            }
            else if (sld_FurnaceType.Text == "Трубчатая")
            {
                furnaceType = Furnace.FurnaceType.TUBE;
            }
            else
            {
                sld_FurnaceType.Text = "Камерная";
                furnaceType = Furnace.FurnaceType.CHAMBER;
            }

            Furnace.CalculateSingleLayerDoor(t1, t0, a3, b3, h3, L1, L2, furnaceType, d0, out t4, out Q2, 
                                             out q2, out x3, out F4, out y2);

            sld_t4Result.Text = Math.Round(t4, 4).ToString();
            sld_Q2Result.Text = Math.Round(Q2, 3).ToString();

            sld_q2SmallResult.Text = Math.Round(q2, 3).ToString();
            sld_x3Result.Text = Math.Round(x3, 3).ToString();
            sld_F4Result.Text = Math.Round(F4, 3).ToString();
            sld_y2Result.Text = Math.Round(y2, 3).ToString();

            try
            {
                thf_Q2Value.Value = (decimal)Q2;
            }
            catch (Exception)
            {

            }
        }

        private void dld_calculateButtonClick(object sender, EventArgs e)
        {
            double t0 = (double)dld_t0Value.Value;
            double t1 = (double)dld_t1Value.Value;
            //double t4 = (double)dld_t4Value.Value;

            double a3 = (double)dld_a3Value.Value;
            double b3 = (double)dld_b3Value.Value;
            double a4 = (double)dld_a4Value.Value;
            double b4 = (double)dld_b4Value.Value;

            double h3 = (double)dld_h3Value.Value;
            double h4 = (double)dld_h4Value.Value;
            double L1 = (double)dld_L1Value.Value;
            double L2 = (double)dld_L2Value.Value;
            double d0 = (double)dld_d0Value.Value;

            double t5;
            double tz;
            double Q2;
            double q2;
            double q2Check;

            double x3, x4, F4, y2;

            Furnace.FurnaceType furnaceType = Furnace.FurnaceType.CHAMBER;

            if (dld_FurnaceType.Text == "Камерная")
            {
                furnaceType = Furnace.FurnaceType.CHAMBER;
            }
            else if (dld_FurnaceType.Text == "Трубчатая")
            {
                furnaceType = Furnace.FurnaceType.TUBE;
            }
            else
            {
                dld_FurnaceType.Text = "Камерная";
                furnaceType = Furnace.FurnaceType.CHAMBER;
            }

            Furnace.CalculateDoubleLayerDoor(t1, t0, a3, b3, a4, b4, h3, h4, L1, L2, furnaceType, d0, out t5, out tz, out Q2, out q2, out q2Check,
                                            out x3, out x4, out F4, out y2);

            dld_t5Result.Text = Math.Round(t5, 3).ToString();
            dld_tzResult.Text = Math.Round(tz, 3).ToString();
            dld_Q2Result.Text = Math.Round(Q2, 3).ToString();
            dld_q2smallResult.Text = Math.Round(q2, 3).ToString();
            //dld_q2CheckResult.Text = Math.Round(q2Check, 3).ToString();

            dld_x3Result.Text = Math.Round(x3, 3).ToString();
            dld_x4Result.Text = Math.Round(x4, 3).ToString();
            dld_F4Result.Text = Math.Round(F4, 3).ToString();
            dld_y2Result.Text = Math.Round(y2, 3).ToString();

            try
            {
                thf_Q2Value.Value = (decimal)Q2;
            }
            catch (Exception)
            {

            }
        }

        private void tf_calculateButtonClick(object sender, EventArgs e)
        {
            double L = (double)tf_LValue.Value;
            double d0 = (double)tf_d0Value.Value;
            double d2 = 0;
            
            double t0 = (double)tf_t0Value.Value;
            double t1 = (double)tf_t1Value.Value;
            double t3 = (double)tf_t3Value.Value;
            
            double h1 = (double)tf_h1Value.Value;
            double h3 = (double)tf_h3Value.Value;
            double h4 = (double)tf_h4Value.Value;
            
            double a1 = (double)tf_a1Value.Value;
            double b1 = (double)tf_b1Value.Value;
            double a2 = (double)tf_a2Value.Value;
            double b2 = (double)tf_b2Value.Value;
            
            
            Furnace.DoorLayerType doorLayerType = Furnace.DoorLayerType.SINGLE_LAYER;

            if (tf_layerType.Text.Equals("Однослойная"))
            {
                doorLayerType = Furnace.DoorLayerType.SINGLE_LAYER;
            }
            else if (tf_layerType.Text.Equals("Двухслойная"))
            {
                doorLayerType = Furnace.DoorLayerType.DOUBLE_LAYER;
            }
            else
            {
                tf_layerType.Text = "Однослойная";
                doorLayerType = Furnace.DoorLayerType.SINGLE_LAYER;
            }

            Furnace.DoorConstructionType doorConstructionType = Furnace.DoorConstructionType.DOOR;

            if (this.tf_doorType.Text.Equals("Дверца"))
            {
                doorConstructionType = Furnace.DoorConstructionType.DOOR;
            }
            else if (this.tf_doorType.Text.Equals("Заглушка"))
            {
                doorConstructionType = Furnace.DoorConstructionType.PLUG;
            }
            else
            {
                this.tf_doorType.Text = "Дверца";
                doorConstructionType = Furnace.DoorConstructionType.DOOR;
            }

            double t2;
            double dz;
            double q1;
            double q1Check;
            double Q1;

            double x1, x2, d1, y1;

            Furnace.CalculateTubeFurnace(L, d0, d2, t0, t1, t3, h1, h3, h4, a1, a2, b1, b2, doorConstructionType, doorLayerType,
                out t2, out dz, out q1, out Q1, out q1Check, out x1, out x2, out d1, out y1);

            tf_t2Result.Text = Math.Round(t2, 3).ToString();
            tf_dzResult.Text = Math.Round(dz, 3).ToString();
            tf_q1smallResult.Text = Math.Round(q1, 3).ToString();
            tf_Q1Result.Text = Math.Round(Q1, 3).ToString();

            tf_x1Result.Text = Math.Round(x1, 3).ToString();
            tf_x2Result.Text = Math.Round(x2, 3).ToString();
            tf_d1Result.Text = Math.Round(d1, 3).ToString();
            tf_y1Result.Text = Math.Round(y1, 3).ToString();
            //tf_q1CheckResult.Text = Math.Round(q1Check, 3).ToString();

            try
            {
                thf_Q1Value.Value = (decimal)Q1;
            }
            catch (Exception)
            {

            }
        }

        //remove
        private void tabPage6_Click(object sender, EventArgs e)
        {

        }

        private void mhc_calculateButtonClick(object sender, EventArgs e)
        {
            Furnace.ConnectionType connection = Furnace.ConnectionType.LINEAR;
            Furnace.HeaterForm form = Furnace.HeaterForm.WIRE;
            Furnace.HeaterArrangement arrangement = Furnace.HeaterArrangement.LINEAR;
            Furnace.HeaterPlacement placement = Furnace.HeaterPlacement.OPEN;
            double Tn = (double)mhc_TnValue.Value;
            double Tm = (double)mhc_TmValue.Value;
            string matName = mhc_matNameValue.Text;
            double Ulinear = (double)mhc_UlinValue.Value;
            double P = (double)mhc_PValue.Value;
            double m = (double)mhc_mValue.Value;

            switch (mhc_connectionValue.Text)
            {
                case ("Линейное"):
                    connection = Furnace.ConnectionType.LINEAR;
                    break;
                case ("Однофазное"):
                    connection = Furnace.ConnectionType.SINGLE_PHASE;
                    break;
                case ("Треугольник"):
                    connection = Furnace.ConnectionType.TRIANGLE;
                    break;
                case ("Звезда"):
                    connection = Furnace.ConnectionType.STAR;
                    break;
                default:
                    connection = Furnace.ConnectionType.LINEAR;
                    mhc_connectionValue.Text = "Линейное";
                    break;
            }

            switch (mhc_heaterFormValue.Text)
            {
                case ("Проволочный"):
                    form = Furnace.HeaterForm.WIRE;
                    break;
                case ("Ленточный"):
                    form = Furnace.HeaterForm.PLATE;
                    break;
                default:
                    form = Furnace.HeaterForm.WIRE;
                    mhc_heaterFormValue.Text = "Проволочный";
                    break;
            }

            switch (mhc_arrangementValue.Text)
            {
                case ("Линейная"):
                    arrangement = Furnace.HeaterArrangement.LINEAR;
                    break;
                case ("Зигзаг"):
                    arrangement = Furnace.HeaterArrangement.ZIGZAG;
                    break;
                case ("Спираль"):
                    arrangement = Furnace.HeaterArrangement.SPIRAL;
                    break;
                default:
                    arrangement = Furnace.HeaterArrangement.LINEAR;
                    mhc_arrangementValue.Text = "Линейная";
                    break;
            }

            switch (mhc_placementValue.Text)
            {
                case ("Открытый"):
                    placement = Furnace.HeaterPlacement.OPEN;
                    break;
                case ("Открытый на трубке"):

                    if (form == Furnace.HeaterForm.WIRE &&
                        arrangement == Furnace.HeaterArrangement.SPIRAL)
                    {
                        placement = Furnace.HeaterPlacement.OPEN_ON_TUBE;
                    }
                    else
                    {
                        placement = Furnace.HeaterPlacement.OPEN;
                        mhc_placementValue.Text = "Открытый";
                    }

                    break;
                case ("На полочке"):
                    placement = Furnace.HeaterPlacement.ON_SHELF;
                    break;
                case ("В пазу"):
                    placement = Furnace.HeaterPlacement.IN_SLOT;
                    break;
                default:
                    placement = Furnace.HeaterPlacement.OPEN;
                    mhc_placementValue.Text = "Открытый";
                    break;
            }

            double Uf;
            double Pf;
            double If;
            double Ilinear;
            double R;
            double LAccurate;
            double LLinear;
            double d;
            double W;
            double Wi;
            string matDebugname;
            double plateBRecommended;
            double plateARecommended;

            Furnace.CalculateMetalHeater(Tn, Tm, matName, Ulinear, P, m, 
                connection, form, arrangement, placement, out Uf, out Pf, out If, out Ilinear, out R,
                out LAccurate, out LLinear, out d, out W, out Wi, out matDebugname, out plateBRecommended, out plateARecommended);

            mhc_UfResult.Text = GetRoundString(Uf, 3);
            mhc_PfResult.Text = GetRoundString(Pf, 3);
            mhc_IfResult.Text = GetRoundString(If, 3);
            mhc_IlinResult.Text = GetRoundString(Ilinear, 3);
            mhc_RResult.Text = GetRoundString(R, 3);
            mhc_LHResult.Text = LAccurate.ToString();
            mhc_LpmResult.Text = LLinear.ToString();
            mhc_daResult.Text = GetRoundString(d, 6);
            mhc_WResult.Text = GetRoundString(W, 3);
            mhc_WiResult.Text = GetRoundString(Wi, 3);
            //mhc_matDebugName.Text = matDebugname;
            mhc_UlinearResult.Text = GetRoundString(Ulinear, 3);
            mhc_bResult.Text = GetRoundString(plateBRecommended, 6);
            mhc_ahResult.Text = GetRoundString(plateARecommended, 6);
        }

        //remove
        private void mhc_PValue_ValueChanged(object sender, EventArgs e)
        {

        }

        private string GetRoundString(double value, int digits)
        {
            return Math.Round(value, digits).ToString();
        }

        private void carborund_CalculateButtonClick(object sender, EventArgs e)
        {
            double Tn = (double)ch_TnValue.Value;
            double Tm = (double)ch_TmValue.Value;
            string matName = ch_heaterType.Text;
            double J = (double)ch_JValue.Value;
            double P = (double)ch_PValue.Value;

            double single_I_sequence = 0;
            double single_U_sequence = 0;
            double single_I_parallel = 0;
            double single_U_parallel = 0;

            double triangle_I_sequence = 0;
            double triangle_U_sequence = 0;
            double triangle_I_parallel = 0;
            double triangle_U_parallel = 0;

            double star_I_sequence = 0;
            double star_U_sequence = 0;
            double star_I_parallel = 0;
            double star_U_parallel = 0;

            double U, Up, P1, W, Wi, I, n, N;

            Furnace.CalculateCarborundHeater(Tn, Tm, matName, J, P,
                                             out single_I_sequence, out single_U_sequence,
                                             out single_I_parallel, out single_U_parallel,

                                             out triangle_I_sequence, out triangle_U_sequence,
                                             out triangle_I_parallel, out triangle_U_parallel,

                                             out star_I_sequence, out star_U_sequence,
                                             out star_I_parallel, out star_U_parallel,
                                             out U, out Up, out I, out N, out n, out P1, out W, out Wi);

            ch_Ilin_seq_sp.Text = single_I_sequence.ToString();
            ch_Ulin_seq_sp.Text = single_U_sequence.ToString();
            ch_Ilin_par_sp.Text = single_I_parallel.ToString();
            ch_Ulin_par_sp.Text = single_U_parallel.ToString();

            ch_Ilin_seq_tr.Text = triangle_I_sequence.ToString();
            ch_Ulin_seq_tr.Text = triangle_U_sequence.ToString();
            ch_Ilin_par_tr.Text = triangle_I_parallel.ToString();
            ch_Ulin_par_tr.Text = triangle_U_parallel.ToString();

            ch_Ilin_seq_st.Text = star_I_sequence.ToString();
            ch_Ulin_seq_st.Text = star_U_sequence.ToString();
            ch_Ilin_par_st.Text = star_I_parallel.ToString();
            ch_Ulin_par_st.Text = star_U_parallel.ToString();

            ch_UResult.Text = U.ToString();
            ch_UpResult.Text = Up.ToString();
            ch_IResult.Text = I.ToString();
            ch_NResult.Text = N.ToString();
            ch_nSmallResult.Text = n.ToString();
            ch_P1Result.Text = P1.ToString();
            ch_WResult.Text = W.ToString();
            ch_WiResult.Text = Wi.ToString();
        }

        private void label63_Click(object sender, EventArgs e)
        {

        }

        private void label55_Click(object sender, EventArgs e)
        {

        }

        private void dmh_CalculateButtonClick(object sender, EventArgs e)
        {
            double Tn = (double)dmh_TnValue.Value;
            double Tm = (double)dmh_TmValue.Value;
            string matName = dmh_HeaterNameValue.Text;
            double P = (double)dmh_PValue.Value;

            double single_I_sequence = 0;
            double single_U_sequence = 0;
            double single_I_parallel = 0;
            double single_U_parallel = 0;

            double triangle_I_sequence = 0;
            double triangle_U_sequence = 0;
            double triangle_I_parallel = 0;
            double triangle_U_parallel = 0;

            double star_I_sequence = 0;
            double star_U_sequence = 0;
            double star_I_parallel = 0;
            double star_U_parallel = 0;

            double U, Up, P1, W, Wi, I, n, N;

            Furnace.CalculateMoSi2Heater(Tn, Tm, matName, P,
                                         out single_I_sequence, out single_U_sequence,
                                         out single_I_parallel, out single_U_parallel,

                                         out triangle_I_sequence, out triangle_U_sequence,
                                         out triangle_I_parallel, out triangle_U_parallel,

                                         out star_I_sequence, out star_U_sequence,
                                         out star_I_parallel, out star_U_parallel,
                                         out U, out Up, out I, out N, out n, out P1, out W, out Wi);

            dmh_single_I_sequence.Text = single_I_sequence.ToString();
            dmh_single_U_sequence.Text = single_U_sequence.ToString();
            dmh_single_I_parallel.Text = single_I_parallel.ToString();
            dmh_single_U_parallel.Text = single_U_parallel.ToString();

            dmh_triangle_I_sequence.Text = triangle_I_sequence.ToString();
            dmh_triangle_U_sequence.Text = triangle_U_sequence.ToString();
            dmh_triangle_I_parallel.Text = triangle_I_parallel.ToString();
            dmh_triangle_U_parallel.Text = triangle_U_parallel.ToString();

            dmh_star_I_sequence.Text = star_I_sequence.ToString();
            dmh_star_U_sequence.Text = star_U_sequence.ToString();
            dmh_star_I_parallel.Text = star_I_parallel.ToString();
            dmh_star_U_parallel.Text = star_U_parallel.ToString();

            dmh_UValue.Text = U.ToString();
            dmh_UpValue.Text = Up.ToString();
            dmh_IValue.Text = I.ToString();
            dmh_NValue.Text = N.ToString();
            dmh_nSmallValue.Text = n.ToString();
            dmh_P1Value.Text = P1.ToString();
            dmh_WValue.Text = W.ToString();
            dmh_WiValue.Text = Wi.ToString();
        }

        private void thf_CalculateButtonClick(object sender, EventArgs e)
        {
            double Q1 = (double)thf_Q1Value.Value;
            double Q2 = (double)thf_Q2Value.Value;

            Furnace.FurnaceHeaterMaterial material = Furnace.FurnaceHeaterMaterial.CARBORUNDUM;

            switch (thf_HeaterType.Text)
            {
                case ("Металлический"):
                    material = Furnace.FurnaceHeaterMaterial.METAL;
                    break;
                case ("Карборундный"):
                    material = Furnace.FurnaceHeaterMaterial.CARBORUNDUM;
                    break;
                case ("ДМ"):
                    material = Furnace.FurnaceHeaterMaterial.MOLYBDENUM_DISILICIDE;
                    break;
                default:
                    thf_HeaterType.Text = "Металлический";
                    material = Furnace.FurnaceHeaterMaterial.METAL;
                    break;
            }

            double Q;
            double P;
            Furnace.CalculateTotalHeatFlux(Q1, Q2, material, out Q, out P);

            thf_QResult.Text = Q.ToString();
            thf_PResult.Text = P.ToString();
        }
    }
}
