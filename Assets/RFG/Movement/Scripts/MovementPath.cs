using UnityEngine;

namespace RFG
{
  public class MovementPath : MonoBehaviour
  {
    public enum State { Loop, PingPong, OneWay };
    public State state = State.OneWay;
    public enum Direction { Forwards, Backwards };
    public Direction direction = Direction.Forwards;
    public bool spawnAtStart = false;
    public Transform[] paths;
    public bool ReachedEnd { get; private set; }
    public bool ReachedStart { get; private set; }

    private int currentIndex = 0;


    private void Start()
    {
      if (spawnAtStart)
      {
        transform.position = GetCurrentTransform().position;
        currentIndex++;
      }
    }

    public Transform GetCurrentTransform()
    {
      return paths[currentIndex];
    }

    public void CheckPath(Transform other)
    {
      Transform currentTransform = GetCurrentTransform();
      int range = (int)Vector2.Distance(currentTransform.position, other.position);
      if (range == 0f)
      {
        NextPath();
      }
    }

    public void Reset()
    {
      ReachedEnd = false;
      ReachedStart = false;
    }

    public void Reverse()
    {
      System.Array.Reverse(paths);
    }

    private void NextPath()
    {
      int nextIndex = direction == Direction.Forwards ? currentIndex + 1 : currentIndex - 1;
      ReachedEnd = nextIndex >= paths.Length;
      ReachedStart = nextIndex < 0;

      if (state == State.PingPong && (ReachedEnd || ReachedStart))
      {
        switch (direction)
        {
          case Direction.Forwards:
            direction = Direction.Backwards;
            nextIndex--;
            break;
          case Direction.Backwards:
            direction = Direction.Forwards;
            nextIndex++;
            break;
          default:
            break;
        }
      }
      else if (state == State.Loop && ReachedEnd)
      {
        nextIndex = 0;
      }
      else if (state == State.Loop && ReachedStart)
      {
        nextIndex = paths.Length - 1;
      }
      else if (ReachedEnd)
      {
        nextIndex = paths.Length - 1;
      }
      else if (ReachedStart)
      {
        nextIndex = 0;
      }
      currentIndex = nextIndex;

    }

  }
}