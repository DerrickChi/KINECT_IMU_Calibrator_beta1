using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;

namespace BluetoothController.WPFUserControls
{
        public partial class IMUQuatViewer : UserControl
        {
            //Set globals
            Quaternion endQuaternion = new Quaternion();
            TranslateTransform3D myTranslation = new TranslateTransform3D();
            //RotateTransform3D baseRotateTransform3D = new RotateTransform3D();
            QuaternionRotation3D startRotation = new QuaternionRotation3D();
            QuaternionRotation3D endRotation = new QuaternionRotation3D();
            Transform3DGroup myprocTransformGroup = new Transform3DGroup();

            //Rotation3DAnimation mydefaultAnimation = new Rotation3DAnimation();
            public IMUQuatViewer ()
            {
                InitializeComponent();

                endRotation.Quaternion = Quaternion.Identity;
                //(new Quaternion(0, 0, 1, 0)) * (new Quaternion(0, 0, 0, 1));
                StartTransfrom();
            }

            public void Update(double w, double x, double y, double z, Quaternion? correction = null)
            {
                if (correction == null)
                    correction = Quaternion.Identity;
                myprocTransformGroup.Children.Clear();

                correction.Value.Normalize();
                endQuaternion = ((Quaternion)correction) * new Quaternion(x, y, z, w);
                //endQuaternion = new Quaternion(x, y, z, w) * (Quaternion)correction;
                endRotation.Quaternion = endQuaternion;
                StartTransfrom();

                this.AxisAngle.Text =  endQuaternion.Axis.ToString(); 
            }
            public void Update(Quaternion? Pos, Quaternion? correction = null)
            {
                this.Update(Pos.Value.W, Pos.Value.X, Pos.Value.Y, Pos.Value.Z, correction);
                //if (correction == null)
                //    correction = Quaternion.Identity;
                //if (Pos == null)
                //    correction = Quaternion.Identity;

                //myprocTransformGroup.Children.Clear();
                ////(new Quaternion(0, 0, 1, 0)) *
                //endQuaternion = Quaternion.Multiply( (Quaternion)correction, (Quaternion) Pos);
                //endRotation.Quaternion = endQuaternion;
                //StartTransfrom();
            }

            int count = 0;
            public void StartTransfrom()
            {
                if (count < 5)
                {
                    count++;
                    return;
                }
                else
                {
                    count = 0;
                    //myprocTransformGroup.Children.Add(baseRotateTransform3D);
                    topModelVisual3D.Transform = new RotateTransform3D(new QuaternionRotation3D(endQuaternion));
                }
            }

        }
}
