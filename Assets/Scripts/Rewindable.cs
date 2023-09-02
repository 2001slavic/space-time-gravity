using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace EnumerableDropOutStack
{
    /// <summary>
    /// An Enumerable Drop Out Stack Class.
    /// </summary>
    /// <typeparam name="T">Some Generic Class</typeparam>
    [Serializable]
    public class EnumerableDropOutStack<T> : IEnumerable<T>
    {

        /// <summary>
        /// The stack collection.
        /// </summary>
        private readonly T[] _items;
        /// <summary>
        /// The top - where the next item will be pushed in the array.
        /// </summary>
        private int _top; // The position in the array that the next item will be placed. 
        /// <summary>
        /// The current number of items in the stack.
        /// </summary>
        private int _count; // The amount of items in the array.

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerableDropOutStack{T}"/> class.
        /// </summary>
        /// <param name="capacity">The capacity of the stack.</param>
        public EnumerableDropOutStack(int capacity)
        {
            _items = new T[capacity];
        }

        /// <summary>
        /// Pushes the specified item onto the stack.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Push(T item)
        {
            _count += 1;
            _count = _count > _items.Length ? _items.Length : _count;

            _items[_top] = item;
            _top = (_top + 1) % _items.Length; // After filling the array the next item will be placed at the beginning of the array!
        }

        /// <summary>
        /// Pops last item from the stack.
        /// </summary>
        /// <returns>T.</returns>
        public T Pop()
        {
            _count -= 1;
            _count = _count < 0 ? 0 : _count;

            _top = (_items.Length + _top - 1) % _items.Length;
            return _items[_top];
        }

        /// <summary>
        /// Peeks at last item on the stack.
        /// </summary>
        /// <returns>T.</returns>
        public T Peek()
        {
            return _items[(_items.Length + _top - 1) % _items.Length]; //Same as pop but without changing the value of top.
        }

        /// <summary>
        /// Returns the amount of elements on the stack.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int Count()
        {
            return _count;
        }

        /// <summary>
        /// Gets an item from the stack.
        /// Index 0 is the last item pushed onto the stack.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>T.</returns>
        /// <exception cref="System.InvalidOperationException">Index out of bounds</exception>
        public T GetItem(int index)
        {
            if (index > Count())
            {
                throw new InvalidOperationException("Index out of bounds");
            }

            else
            {
                // The first element = last element entered = index 0 is at Peek - see above.
                // index 0 = items[(items.Length + top - 1) % items.Length];
                // index 1 = items[(items.Length + top - 2) % items.Length];
                // index 2 = items[(items.Length + top - 3) % items.Length]; etc...
                // So to get an item at a certain index is:
                // items[(items.Length + top - (index+1)) % items.Length];

                return _items[(_items.Length + _top - (index + 1)) % _items.Length];
            }
        }

        /// <summary>
        /// Clears the stack.
        /// </summary>
        public void Clear()
        {
            _count = 0;
        }

        /// <summary>
        /// Returns an enumerator for a generic stack that iterates through the stack.
        /// The iterator start at the last item pushed onto the stack and goes backwards.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count(); i++)
            {
                yield return GetItem(i);
            }
        }


        /// <exclude />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

public class Rewindable : MonoBehaviour
{
    public TimeControl timeControl;
    public Rigidbody rb;
    public Light light;

    public static int HISTORY_STACK_SIZE;
    private EnumerableDropOutStack.EnumerableDropOutStack<Vector3> positionHistory;
    private EnumerableDropOutStack.EnumerableDropOutStack<Quaternion> rotationHistory;
    private EnumerableDropOutStack.EnumerableDropOutStack<Vector3> rbVelocityHistory;
    private EnumerableDropOutStack.EnumerableDropOutStack<Material> materialHistory;
    private EnumerableDropOutStack.EnumerableDropOutStack<bool> lightStateHistory;
    private Vector3 freezePos;
    private Quaternion freezeRotation;
    private Vector3 freezeRbVelocity;
    private Material freezeMaterial;
    private bool freezeLightState;
    public bool waitingForPause;
    void Start()
    {
        HISTORY_STACK_SIZE = Mathf.RoundToInt(2000 * timeControl.effectMaxTime);
        rb = GetComponent<Rigidbody>();
        if (rb != null && !rb.isKinematic)
            rbVelocityHistory = new EnumerableDropOutStack.EnumerableDropOutStack<Vector3>(HISTORY_STACK_SIZE);
        light = GetComponent<Light>();
        if (light != null)
            lightStateHistory = new EnumerableDropOutStack.EnumerableDropOutStack<bool>(HISTORY_STACK_SIZE);
        positionHistory = new EnumerableDropOutStack.EnumerableDropOutStack<Vector3>(HISTORY_STACK_SIZE);
        rotationHistory = new EnumerableDropOutStack.EnumerableDropOutStack<Quaternion>(HISTORY_STACK_SIZE);
        materialHistory = new EnumerableDropOutStack.EnumerableDropOutStack<Material>(HISTORY_STACK_SIZE);
        waitingForPause = true;
        freezePos = transform.position;
    }

    void Update()
    {
        if (timeControl.pauseOn)
        {
            if (waitingForPause)
            {
                freezePos = transform.position;
                freezeRotation = transform.rotation;
                freezeMaterial = gameObject.GetComponent<Renderer>().material;
                if (rb != null && !rb.isKinematic)
                    freezeRbVelocity = rb.velocity;
                if (light != null)
                    freezeLightState = light.enabled;
                waitingForPause = false;
            }
            transform.SetPositionAndRotation(freezePos, freezeRotation);
            gameObject.GetComponent<Renderer>().material = freezeMaterial;
            if (rb != null && !rb.isKinematic)
                rb.velocity = freezeRbVelocity;
            if (light != null)
                light.enabled = freezeLightState;
        }
        else
        {
            waitingForPause = true;
        }

        if (timeControl.pauseOn)
            ;
        else if (!timeControl.rewindOn)
        {
            positionHistory.Push(transform.position);
            rotationHistory.Push(transform.rotation);
            materialHistory.Push(gameObject.GetComponent<Renderer>().material);
            if (rb != null && !rb.isKinematic)
                rbVelocityHistory.Push(rb.velocity);
            if (light != null)
                lightStateHistory.Push(light.enabled);
        }
        else if (timeControl.rewindOn)
        {
            if (positionHistory.Count() == 0)
            {
                timeControl.rewindOn = false;
            }
            else
            {
                transform.SetPositionAndRotation(positionHistory.Pop(), rotationHistory.Pop());
                gameObject.GetComponent<Renderer>().material = materialHistory.Pop();
                if (rb != null && !rb.isKinematic)
                    rb.velocity = rbVelocityHistory.Pop();
                if (light != null)
                    light.enabled = lightStateHistory.Pop();
            }
        }
    }
}
