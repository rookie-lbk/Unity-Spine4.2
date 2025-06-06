using UnityEngine;

public class TargetCtrl : MonoBehaviour
{
    public LayerMask mask;

    public void OnGUI()
    {
        if (Event.current.clickCount == 1)
        {
            UpdateTargetPosition();
        }
    }

    public void UpdateTargetPosition()
    {
        Vector3 newPosition = Vector3.zero;
        bool positionFound = false;

        // If the game view has never been rendered, the mouse position can be infinite
        if (!float.IsFinite(Input.mousePosition.x)) return;

        newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newPosition.z = 0;
        positionFound = true;
        var collider = Physics2D.OverlapPoint(newPosition, mask);
        if (collider != null)
        {
            positionFound = false;
        }

        if (positionFound)
        {
            transform.position = newPosition;
        }
    }
}
