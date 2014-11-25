using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace QuaternionView
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : UserControl
    {
        //Set globals
        Quaternion startQuaternion = new Quaternion(1,0,0,0);
        Quaternion endQuaternion = new Quaternion();
        TranslateTransform3D myTranslation = new TranslateTransform3D();
        RotateTransform3D baseRotateTransform3D = new RotateTransform3D();
        QuaternionRotation3D startRotation = new QuaternionRotation3D();
        QuaternionRotation3D endRotation = new QuaternionRotation3D();
        Transform3DGroup myprocTransformGroup = new Transform3DGroup();
        Rotation3DAnimation mydefaultAnimation = new Rotation3DAnimation();       
        public Window1()
        {
            InitializeComponent();
            endRotation.Quaternion = Quaternion.Identity * (new Quaternion(0, 0, 1, 0));
                //(new Quaternion(0, 0, 1, 0)) * (new Quaternion(0, 0, 0, 1));
            startAnimation();
        }

        public void Update(double w, double x, double y, double z, Quaternion? correction = null)
        {
            if (correction == null)
                correction = Quaternion.Identity;
            myprocTransformGroup.Children.Clear();
            endQuaternion = Quaternion.Multiply((Quaternion)correction,  new Quaternion(x,y, z, w));
            endRotation.Quaternion = endQuaternion;
            startAnimation();
        }

        public void startAnimation()
        {
            mydefaultAnimation.From = startRotation;
            mydefaultAnimation.To = endRotation;
            mydefaultAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(10));
            mydefaultAnimation.FillBehavior = FillBehavior.HoldEnd;

            baseRotateTransform3D.BeginAnimation(RotateTransform3D.RotationProperty, mydefaultAnimation);
            myprocTransformGroup.Children.Add(baseRotateTransform3D);
            topModelVisual3D.Transform = myprocTransformGroup;

            //Update text boxes
            //TransformMatrix.Text = myprocTransformGroup.Value.ToString();
            //TransformMatrix.Text = baseRotateTransform3D.Value.ToString();

            startRotation = endRotation;
            //resulting string is (x,y,z,w)

        }

    }
}