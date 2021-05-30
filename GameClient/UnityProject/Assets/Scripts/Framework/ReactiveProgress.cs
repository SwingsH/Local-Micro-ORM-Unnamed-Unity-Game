using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace TIZSoft
{

    public struct ProgressValueSet
    {
        public float Progress
        {
            get
            {
                if (TotalAmount != 0)
                {
                    return 1 - (float)RestAmount / TotalAmount;
                }
                else
                    return 0;
            }
        }
        public int RestAmount;
        public int TotalAmount;
        public int ErrorAmount;
    }

    public class ReactiveProgress : ISubject<ProgressValueSet>
    {
        Subject<ProgressValueSet> publisher = new Subject<ProgressValueSet>();
        ProgressValueSet progressValueSet;
        
        public float Progress
        {
            get
            {
                return progressValueSet.Progress;
            }
        }
        public int RestAmount
        {
            get
            {
                return progressValueSet.RestAmount;
            }
            set
            {
                progressValueSet.RestAmount = value;
                OnNext(progressValueSet);
            }
        }
        public int TotalAmount
        {
            get
            {
                return progressValueSet.TotalAmount;
            }
            set
            {
                progressValueSet.TotalAmount = value;
                OnNext(progressValueSet);
            }
        }
        public int ErrorAmount
        {
            get
            {
                return progressValueSet.ErrorAmount;
            }
            set
            {
                progressValueSet.ErrorAmount = value;
                OnNext(progressValueSet);
            }
        }
        public ReactiveProgress()
        {
        }
        public IDisposable Subscribe(IObserver<ProgressValueSet> observer)
        {
            var subscription = publisher.Subscribe(observer);
            observer.OnNext(progressValueSet);
            return subscription;
        }

        public void OnCompleted()
        {
            publisher.OnCompleted();
        }

        public void OnError(Exception error)
        {
            publisher.OnError(error);
        }

        public void OnNext(ProgressValueSet value)
        {
            publisher.OnNext(value);
        }
    }
}
