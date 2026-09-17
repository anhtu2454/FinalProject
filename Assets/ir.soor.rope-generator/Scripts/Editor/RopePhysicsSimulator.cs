using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Soor.RopeGenerator.Editor
{
    public static class RopePhysicsSimulator
    {
        /// <summary>
        /// Giữ biến này để không phá code gọi đến từ RopeEditorWindow.cs.
        /// </summary>
        public static bool registered = false;

        /// <summary>
        /// ĐÃ VÔ HIỆU HÓA việc tự hook vào EditorApplication.update.
        /// Lý do: Physics.Simulate() luôn mô phỏng TOÀN BỘ scene (không có cách giới hạn
        /// chỉ chạy cho riêng các đốt dây) -- nên chỉ cần mở cửa sổ Rope Editor lên là mọi
        /// Rigidbody khác trong scene (không chỉ dây) cũng bị rơi theo gravity ngay trong
        /// Edit Mode, dù chưa bấm Play. Vật lý gameplay thật vẫn chạy đúng và an toàn khi
        /// bấm Play (Unity tự lo), không cần cơ chế "preview trong Edit Mode" này.
        /// </summary>
        public static void Register()
        {
            registered = true;
        }

        /// <summary>
        /// Bật vật lý CHỈ cho các Rigidbody thuộc đốt dây (tag "RopeSegment").
        /// Trong Edit Mode sẽ không thấy hiệu ứng gì (Unity không tự chạy physics khi
        /// chưa Play) -- đây là điều bình thường và AN TOÀN, không phải lỗi.
        /// </summary>
        public static void Activate()
        {
            var allRopeSegments = GetAllRopeSegmentRigidbodies();
            foreach (var rb in allRopeSegments)
            {
                rb.isKinematic = false;
                rb.WakeUp();
            }
            Debug.unityLogger.Log("Rope physics simulation started (chỉ các đốt dây). Chỉ thấy hiệu ứng thật khi bấm Play.");
        }

        /// <summary>
        /// Dừng vật lý CHỈ cho các Rigidbody thuộc đốt dây (đóng băng bằng isKinematic).
        /// </summary>
        public static void Deactivate()
        {
            var allRopeSegments = GetAllRopeSegmentRigidbodies();
            foreach (var rb in allRopeSegments)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
            Debug.unityLogger.Log("Rope physics simulation stopped (chỉ các đốt dây).");
        }

        /// <summary>
        /// Finds all rigidbodies in the scene that belong to objects tagged as "RopeSegment" and returns them as a list.
        /// </summary>
        private static List<Rigidbody> GetAllRopeSegmentRigidbodies()
        {
            var allRopeSegments = Object.FindObjectsOfType<Rigidbody>().ToList();
            allRopeSegments = allRopeSegments.Where(rb => rb.gameObject.CompareTag("RopeSegment")).ToList();
            return allRopeSegments;
        }
    }
}