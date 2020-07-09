using UnityEngine;

namespace Player.RopeMechanics
{
    public class RopeRender : MonoBehaviour
    {
        public LineRenderer line;
        private int indexSrc;
        private int indexEnd;

        private void Start()
        {
            line.positionCount = 2;
            indexSrc = 0;
            indexEnd = 1;
        }

        public void DrawRope(Vector2 src, Vector2 end)
        {
            line.enabled = true;
            line.SetPosition(indexSrc,src);            
            line.SetPosition(indexEnd,end);            
        }

        public void HideRope()
        {
            line.enabled = false;
        }
    }
}