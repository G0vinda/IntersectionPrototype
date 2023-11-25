using Cinemachine;
using UnityEngine;

namespace Camera
{
    public class CameraClamp : CinemachineExtension
    {
        [SerializeField] private float xMin;
        [SerializeField] private float xMax;

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            var pos = state.RawPosition;
            pos.x = Mathf.Clamp(pos.x, xMin, xMax);
            state.RawPosition = pos;
        }
    }
}