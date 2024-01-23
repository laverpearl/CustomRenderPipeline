//using System;
using UnityEngine;

public class MeshBall : MonoBehaviour
{
    private static int baseColorId = Shader.PropertyToID("_BaseColor"),
    metallicId = Shader.PropertyToID("_Metallic"),
	smoothnessId = Shader.PropertyToID("_Smoothness");

    [SerializeField]
    private Mesh mesh = default;

    [SerializeField]
    private Material material = default;
    private Matrix4x4[] matrices = new Matrix4x4[100];
    private Vector4[] baseColors = new Vector4[100];
    float[] metallic = new float[100], smoothness = new float[100];
    private MaterialPropertyBlock block;

    //private void Awake()
    //{
    //    for (int i = 0; i < this.matrices.Length; i++)
    //    {
    //        this.matrices[i] = Matrix4x4.TRS(
    //          Random.insideUnitSphere * 20f, 
    //          Quaternion.identity, Vector3.one);
    //        this.baseColors[i] =
    //          new Vector4(Random.value, Random.value, Random.value, 1f);
    //    }
    //}
    void Awake()
    {
        for (int i = 0; i < matrices.Length; i++)
        {
            matrices[i] = Matrix4x4.TRS(
                Random.insideUnitSphere * 5f,
                Quaternion.Euler(
                    Random.value * 360f, Random.value * 360f, Random.value * 360f
                ),
                Vector3.one * Random.Range(0.5f, 1.5f)
            );
            //baseColors[i] =
            //    new Vector4(
            //        Random.value, Random.value, Random.value,
            //        Random.Range(0.5f, 1f)
            //    );
            baseColors[i] = new Vector4(1f,1f,1f,1f);//new Vector4(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f));
            metallic[i] = 0.2f;//Random.value < 0.25f ? 1f : 0f;
            smoothness[i] = 0.9f;//Random.Range(0.05f, 0.95f);
        }
    }

    //private void Update()
    //{
    //    if (this.block == null)
    //    {
    //        this.block = new MaterialPropertyBlock();
    //        this.block.SetVectorArray(baseColorId, this.baseColors);
    //    }
    //    Graphics.DrawMeshInstanced(this.mesh, 0, this.material, this.matrices, 1023, this.block);
    //}
    void Update()
    {
        if (block == null)
        {
            block = new MaterialPropertyBlock();
            block.SetVectorArray(baseColorId, baseColors);
            block.SetFloatArray(metallicId, metallic);
            block.SetFloatArray(smoothnessId, smoothness);
        }
        Graphics.DrawMeshInstanced(mesh, 0, material, matrices, 100, block);
    }
}