using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cam;

    void Start()
    {
        // Ana kamerayı bul ve 'cam' değişkenine ata
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        // Canvas'ın rotasyonunu, kameranın rotasyonuyla aynı yap
        transform.rotation = cam.rotation;
        
        // VEYA (Alternatif - 3D bir oyundaysanız bu daha iyi çalışır):
        // transform.LookAt(transform.position + cam.forward);
    }
}