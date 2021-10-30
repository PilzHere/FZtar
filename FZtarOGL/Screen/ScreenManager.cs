using System;
using System.Collections.Generic;
using System.Linq;

namespace FZtarOGL.Screen
{
    public class ScreenManager
    {
        private Stack<Screen> _gameScreens;
        private Screen _tempNewScreen;
        
        private bool _loadScreen, _removeScreen;

        public Stack<Screen> GameScreens => _gameScreens;

        public ScreenManager()
        {
            _gameScreens = new Stack<Screen>();
        }

        public void Tick()
        {
            if (_loadScreen)
            {
                AddScreenInternal();
            } else if (_removeScreen)
            {
                RemoveScreenInternal();
            }
        }

        /// <summary>
        /// Will execute at NEXT tick.
        /// </summary>
        public void AddScreen(Screen newScreen)
        {
            _tempNewScreen = newScreen;
            _loadScreen = true;
        }

        private void AddScreenInternal()
        {
            if (_tempNewScreen != null)
            {
                _gameScreens.Push(_tempNewScreen);
            }

            _tempNewScreen = null;
            _loadScreen = false;
        }

        /// <summary>
        /// Will execute at NEXT tick.
        /// </summary>
        public void RemoveScreen()
        {
            _removeScreen = true;
        }

        private void RemoveScreenInternal()
        {
            _gameScreens.First().Destroy();
            _gameScreens.Pop();
            _removeScreen = false;
        }
        
        /// <summary>
        /// Removes all current screens.
        /// </summary>
        public void RemoveAllScreens()
        {
            while (_gameScreens.Count != 0)
            {
                _gameScreens.First().Destroy();
                _gameScreens.Pop();
            }

            _gameScreens.Clear();

            //_removeAllScreens = false;
            
            Console.WriteLine("Removed all screens.");
        }
    }
}