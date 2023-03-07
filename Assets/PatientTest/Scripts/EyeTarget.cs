using UnityEngine;
using UnityEngine.Events;

namespace PatientTest.Scripts
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class EyeTarget : MonoBehaviour
    {

        public bool IsHovered{ get; set; }

        [SerializeField]
        private UnityEvent<GameObject> OnObjectHover;

        [SerializeField]
        private Material OnHoverActiveMaterial;

        [SerializeField]
        private Material OnHoverInactiveMaterial;

        private MeshRenderer meshRenderer;

        // Start is called before the first frame update
        void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();    
        }

        // Update is called once per frame
        void Update()
        {
            if(IsHovered)
            {
                meshRenderer.material = OnHoverActiveMaterial;
                //Debug.Log("Yes");
                OnObjectHover?.Invoke(gameObject);
            }

            else
            {
                //Debug.Log("No");
                meshRenderer.material = OnHoverInactiveMaterial;
            }
        }
    }
}
