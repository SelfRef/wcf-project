using Newtonsoft.Json;

namespace WCFReference
{
    public class Controll
    {
        public enum Direction
        {
            Front,
            Back
        }
        public enum Side
        {
            Left,
            Right
        }
        public bool Front { get; set; } = false;
        public bool Back { get; set; } = false;
        public bool Left { get; set; } = false;
        public bool Right { get; set; } = false;
        public bool Brake { get; set; } = false;
        public bool Boost { get; set; } = false;

        public bool AnyMove
        {
            get
            {
                return (Front || Back || Left || Right);
            }
        }
        public Controll(Direction? dir, Side? side, bool? brake, bool? boost)
        {
            if (dir == Direction.Front)
            {
                Front = true;
            }
            else if(dir == Direction.Back)
            {
                Back = true;
            }
            if(side == Side.Left)
            {
                Left = true;
            }
            else if(side == Side.Right)
            {
                Right = true;
            }
        }

        public Controll(bool dir, bool side, bool? brake, bool? boost)
        {
            Front = dir;
            Back = !dir;
            Left = side;
            Right = !side;

            Brake = brake ?? false;
            Boost = boost ?? false;
        }

        [JsonConstructor]
        public Controll(bool front, bool back, bool left, bool right, bool brake = false, bool boost = false)
        {
            if(front && !back)
            {
                Front = true;
                Back = false;
            }
            else if(!front && back)
            {
                Front = false;
                Back = true;
            }
            else
            {
                Front = false;
                Back = false;
            }

            if (left && !right)
            {
                Left = true;
                Right = false;
            }
            else if (!left && right)
            {
                Left = false;
                Right = true;
            }
            else
            {
                Left = false;
                Right = false;
            }

            Brake = brake;
            Boost = boost;
        }
    }
}
