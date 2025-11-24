using UnityEngine;
using UnityEngine.UIElements;

namespace Interfaces
{
    public interface IDamageable
    {
        public void TakeDamage(int amount, Vector3 position);
    }
}