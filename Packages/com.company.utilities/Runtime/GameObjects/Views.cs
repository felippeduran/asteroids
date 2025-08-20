using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace Company.Utilities.Runtime
{
    public interface IView<TState, TEvents>
    {
        void UpdateView(TState state);
        void RegisterEvents(TEvents events);
    }

    public class ViewController<TInstance> where TInstance : MonoBehaviour
    {
        readonly TInstance obj;
        readonly AssetReferenceGameObject assetRef;

        public TInstance Obj => obj;

        public ViewController(AssetReferenceGameObject assetRef)
        {
            this.obj = obj;
            this.assetRef = assetRef;
        }

        public void Dispose()
        {
            assetRef.ReleaseInstance(obj.gameObject);
        }
    }


    public class ViewBinding<TView, TState, TEvents> where TView : IView<TState, TEvents>
    {
        TView view;
        TState state;
        TEvents events;

        public ViewBinding(TView view, TState state, TEvents events)
        {
            this.view = view;
            this.state = state;
            this.events = events;
        }

        public void Present()
        {
            view.UpdateView(state);
            view.RegisterEvents(events);
        }

        public void Dismiss()
        {
            view.UpdateView(state);
        }
    }

    public class ViewsOrchestrator
    {
        Stack<ViewBinding<AView, AState, IAEvents>> views = new Stack<ViewBinding<AView, AState, IAEvents>>();

        async void Update()
        {
            views
        }

        public void AddView(ViewBinding<AView, AState, IAEvents> view)
        {
            views.Push(view);
        }
    }


    public class CoreState
    {
        public float Value1 { get; set; }
        public string Value2 { get; set; }
        public List<int> Value3 { get; set; }
    }

    [Serializable]
    public struct AState
    {
        public float FieldA { get; set; }
        public string FieldB { get; set; }
    }

    public interface IAEvents
    {
        void ButtonClick1();
        void SelectIndex(int index);
    }

    public class AView : MonoBehaviour, IView<AState, IAEvents>
    {
        [SerializeField] TMP_Text text;
        [SerializeField] Button button1;
        [SerializeField] Button button2;

        Action<int> click2;

        public void UpdateView(AState state)
        {
            text.text = state.ToString();
        }

        public void RegisterEvents(IAEvents events)
        {
            button1.onClick.AddListener(events.ButtonClick1);
            click2 = events.SelectIndex;
        }

        public void Update()
        {
            click2?.Invoke(1);
        }
    }

    public class AEventsHandler : IAEvents
    {
        public Action<int> OnSelectIndex;
        public Action OnButtonClick1;

        public void ButtonClick1()
        {
            OnButtonClick1?.Invoke();
        }

        public void SelectIndex(int index)
        {
            OnSelectIndex?.Invoke(index);
        }
    }

    public interface IViewOrchestrator
    {
        void PushView(ViewBinding<AView, AState, IAEvents> view);
        void PopView(ViewBinding<AView, AState, IAEvents> view);
        void ResetView();
    }

    public class Example : MonoBehaviour
    {
        public void Start()
        {
            var state = new CoreState();

            var orchestrator = new ViewsOrchestrator();

            orchestrator.AddView(A(state));
        }

        public ViewBinding<AView, AState, IAEvents> A(CoreState state)
        {
            return new ViewBinding<AView, AState, IAEvents>(new AView(), new AState
            {
                FieldA = state.Value1,
                FieldB = string.Join(", ", state.Value3.Select(x => x.ToString())),
            }, new AEventsHandler
            {
                OnButtonClick1 = () => { },
                OnSelectIndex = (index) => { },
            });
        }

        public ViewBinding<AView, AState, IAEvents> B(CoreState state)
        {
            return new ViewBinding<AView, AState, IAEvents>(new AView(), new AState
            {
                FieldA = state.Value1,
                FieldB = string.Join(", ", state.Value3.Select(x => x.ToString())),
            }, new AEventsHandler
            {
                OnButtonClick1 = () => { orchestrator.AddView(A(state)); },
                OnSelectIndex = (index) => { },
            });
        }

    }
}