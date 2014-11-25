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
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BluetoothController.WPFUserControls
{
    /// <summary>
    /// Interaction logic for QuaternionArrow.xaml
    /// </summary>
    public partial class QuaternionArrow : UserControl
    {
        public QuaternionArrow()
        {
            InitializeComponent();
        }
                //Set globals
        Quaternion startQuaternion = new Quaternion(0,0,0,1);
        Quaternion endQuaternion = new Quaternion();
        TranslateTransform3D myTranslation = new TranslateTransform3D();
        RotateTransform3D baseRotateTransform3D = new RotateTransform3D();
        QuaternionRotation3D startRotation = new QuaternionRotation3D();
        QuaternionRotation3D endRotation = new QuaternionRotation3D();
        Transform3DGroup myprocTransformGroup = new Transform3DGroup();
        //Rotation3DAnimation mydefaultAnimation = new Rotation3DAnimation();       


        public void Update(double w, double x, double y, double z, System.Windows.Media.Media3D.Quaternion? correction = null)
        {
            if (correction == null)
                correction = Quaternion.Identity;
            myprocTransformGroup.Children.Clear();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("w{0},x{1},y{2},z{3}",
                w.ToString("#.000"),
                x.ToString("#.000"),
                y.ToString("#.000"),
                z.ToString("#.000")
                );
            this.Kinect.Text = sb.ToString();
            endQuaternion = Quaternion.Multiply((Quaternion)correction,  new Quaternion(x,y, z, w));
            endRotation.Quaternion = endQuaternion;
            startAnimation();
        }
        public void Update(Quaternion? q, Quaternion? correction = null)
        {
            if (correction == null)
                correction = Quaternion.Identity;
            if (q == null)
                q = Quaternion.Identity;
            myprocTransformGroup.Children.Clear();
            //new Quaternion(0, 0, 0, 1)) * (new Quaternion(Math.Sqrt(.5), 0, 0, Math.Sqrt(.5)))
            //(new Quaternion(0, 0, 0, 1)) * (new Quaternion(Math.Sqrt(.5), 0, 0, Math.Sqrt(.5))) *
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("w{0},x{1},y{2},z{3}",
                q.Value.W.ToString("#.000"),
                q.Value.X.ToString("#.000"),
                q.Value.Y.ToString("#.000"),
                q.Value.Z.ToString("#.000")
                );
            this.Kinect.Text = sb.ToString();
            endQuaternion =  ((Quaternion)correction) * ((Quaternion)q);
            endRotation.Quaternion = endQuaternion;
            startAnimation();
        }

        public void startAnimation()
        {
            //mydefaultAnimation.From = startRotation;
            //mydefaultAnimation.To = endRotation;
            //mydefaultAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(10));
            //mydefaultAnimation.FillBehavior = FillBehavior.HoldEnd;

           // baseRotateTransform3D.BeginAnimation(RotateTransform3D.RotationProperty, mydefaultAnimation);
            //myprocTransformGroup.Children.Add(baseRotateTransform3D);
            this.Cord.Transform = new RotateTransform3D(endRotation);
            //this.Arrow.Transform = myprocTransformGroup;

            //Update text boxes
            //TransformMatrix.Text = myprocTransformGroup.Value.ToString();
            //TransformMatrix.Text = baseRotateTransform3D.Value.ToString();

            //startRotation = endRotation;
            //StringBuilder sb = new StringBuilder();
            //sb.AppendFormat("w{0},x{1},y{2},z{3}",
            //    endQuaternion.W,
            //    endQuaternion.X,
            //    endQuaternion.Y,
            //    endQuaternion.Z
            //    );
            //this.Kinect.Text = sb.ToString();
            //resulting string is (x,y,z,w)

        }

    }
}
