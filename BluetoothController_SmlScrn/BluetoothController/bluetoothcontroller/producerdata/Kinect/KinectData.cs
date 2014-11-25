using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace BluetoothController.Kinect
{
    public class KinectData : IData
    {
        protected JointType[] _JointsOfInterest;
        protected IReadOnlyDictionary<JointType, Joint> _Joints;
        public Body Body;
        public long NowInTicks { get; set; }
        public KinectData(JointType[] jointsofinterest, Body body)
        {
            NowInTicks = System.DateTime.UtcNow.Ticks;
            _JointsOfInterest = jointsofinterest;
            Body = body;
            _Joints = body.Joints;
        }
        public string JointsOfInterestToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (JointType joint in _JointsOfInterest)
            {
                builder.AppendFormat("{0}, X: {1}, Y: {2}, Z:{3}\r\n",
                    joint.ToString(),
                    _Joints[joint].Position.X,
                    _Joints[joint].Position.Y,
                    _Joints[joint].Position.Z);
            }
            return builder.ToString();
        }
        string IData.ToPreviewString()
        {
            return this.JointsOfInterestToString();
        }
        public IDictionary<JointType, Joint> JointsOfInterest
        {
            get
            {
                Dictionary<JointType, Joint> joi = new Dictionary<JointType, Joint>();
                foreach (JointType jtype in _JointsOfInterest)
                {
                    joi.Add(jtype, _Joints[jtype]);
                }
                return joi;
            }

        }
        //public Body Body
        //{
        //    get
        //    {
        //        return Body;
        //    }
        //}
        public IDictionary<JointType, Joint> Joints
        {
            get
            {
                return (IDictionary<JointType, Joint>) _Joints;
            }
        }

        string IData.ToWindowsFormString()
        {
            throw new NotImplementedException();
        }

        string IData.ToFileString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (JointType joint in Joints.Keys)
            {
                sb.AppendFormat("ID{0}, ", Body.TrackingId);
                sb.AppendFormat("Time{0}, ", this.NowInTicks);
                sb.AppendFormat("x{0}, y{1}, z{2}, ",
                    Joints[joint].Position.X,
                    Joints[joint].Position.Y,
                    Joints[joint].Position.Z);
                //This will add in the negative sign if the body is being infered   
                if (Joints[joint].TrackingState == TrackingState.Inferred)
                    sb.Append("-");
                //this will get the numeric value of the jointtype hehehe
                sb.Append(Convert.ChangeType(Joints[joint].JointType, Joints[joint].JointType.GetTypeCode()));
                sb.AppendLine();
            }
            return sb.ToString();
        }

        #region IData Members


        public string ToFileHeaderString()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}