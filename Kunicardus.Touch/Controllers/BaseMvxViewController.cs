﻿using System;
using Cirrious.MvvmCross.Touch.Views;
using UIKit;
using Kunicardus.Core.Converters;
using Foundation;
using CoreGraphics;

namespace Kunicardus.Touch
{
	public class BaseMvxViewController : MvxViewController
	{
		#region Keyboard Controlling Vars

		private UIView activeview;
		// Controller that activated the keyboard
		private nfloat scroll_amount = 0.0f;
		// amount to scroll
		private nfloat bottom = 0.0f;
		// bottom point
		private nfloat offset = 10.0f;
		// extra offset
		private bool moveViewUp = false;
		// which direction are we moving
		private nfloat startingY = 0;


		#endregion

		public static AppDelegate APP {
			get { 
				return UIApplication.SharedApplication.Delegate as AppDelegate;
			}
		}

		public bool ScrollViewOnKeyboardShow {
			get;
			set;
		}


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NavigationController.NavigationBar.TintColor = UIColor.White;
			ShowMenuIcon ();

			#region Keyboard controlling
			if (ScrollViewOnKeyboardShow) {
				// Keyboard popup
				NSNotificationCenter.DefaultCenter.AddObserver
				(UIKeyboard.DidShowNotification, KeyBoardUpNotification);

				// Keyboard Down
				NSNotificationCenter.DefaultCenter.AddObserver
				(UIKeyboard.WillHideNotification, KeyBoardDownNotification);
			}
			#endregion
			startingY = View.Frame.Y + (!NavigationController.NavigationBar.Translucent ? GetStatusBarHeight () : 0);
		}

		public override void ViewWillAppear (bool animated)
		{			
			base.ViewWillAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public override void ViewWillUnload ()
		{
			base.ViewWillUnload ();
		}

		private bool _buttonAdded;

		protected void ShowMenuIcon ()
		{
			if (!HideMenuIcon) {
				AppDelegate app = UIApplication.SharedApplication.Delegate as AppDelegate;
				NavigationItem.SetLeftBarButtonItem (
					new UIBarButtonItem (UIImage.FromBundle ("threelines")
						, UIBarButtonItemStyle.Plain
						, (sender, args) => app.SidebarController.ToggleMenu ()), true);

				UIBarButtonItem.Appearance.SetBackButtonTitlePositionAdjustment (new UIOffset (0, 0), UIBarMetrics.Default);
			} else {
				if (!_buttonAdded) {
					this.NavigationItem.Title = "";
					UIBarButtonItem backButton = new UIBarButtonItem ("", UIBarButtonItemStyle.Plain, null, null);
					this.NavigationItem.BackBarButtonItem = backButton;
					_buttonAdded = true;
				}
				if (!UIDevice.CurrentDevice.CheckSystemVersion (9, 0)) {
					UIBarButtonItem.Appearance.SetBackButtonTitlePositionAdjustment (new UIOffset (0, -60), UIBarMetrics.Default);	
				}
			}
		}

		public nfloat GetStatusBarHeight ()
		{			
			nfloat statusBarInfoHeight = UIApplication.SharedApplication.StatusBarFrame.Height;			
			if (statusBarInfoHeight < 20) {
				statusBarInfoHeight = 20;
			}
			nfloat navBarHeight = 44;
			try {
				navBarHeight = this.NavigationController.NavigationBar.Frame.Height;
			} catch {
			}
			return (statusBarInfoHeight + navBarHeight);
		}

		public bool HideMenuIcon {
			get;
			set;
		}

		#region Keyboard Controlling Methods

		private void KeyBoardUpNotification (NSNotification notification)
		{
			activeview = null;
			// get the keyboard size
			CGRect r = UIKeyboard.BoundsFromNotification (notification);

			// Find what opened the keyboard
			foreach (UIView view in this.View.Subviews) {
				if (view.GetType () == typeof(KuniTextField)) {
					if (((KuniTextField)view).Field.IsFirstResponder) {
						activeview = view;
						break;
					}
				}
			}
			// Second Try
			if (activeview == null) {
				foreach (UIView view in this.View.Subviews) {
					if (view.GetType () == typeof(UITextField)) {
						if (((UITextField)view).IsFirstResponder) {
							activeview = view;
							break;
						}
					}
				}
			}
			// Third try
			if (activeview == null) {
				foreach (UIView view in this.View.Subviews) {
					foreach (UIView subview in view) {
						if (subview.GetType () == typeof(UITextField)) {
							if (((UITextField)subview).IsFirstResponder) {
								activeview = subview;
								break;
							}
						}
					}
				}
			}
			if (activeview == null)
				return;

			//nfloat extra = 
			// Bottom of the controller = initial position + height + offset      
			bottom = (activeview.Frame.Y + activeview.Frame.Height + offset);

			// Calculate how far we need to scroll
			scroll_amount = (r.Height - (View.Frame.Size.Height - bottom));

			// Perform the scrolling
			if (scroll_amount > 0) {
				moveViewUp = true;
				ScrollTheView (moveViewUp);
			} else {
				moveViewUp = false;
			}

		}

		private void KeyBoardDownNotification (NSNotification notification)
		{
			if (moveViewUp) {
				ScrollTheView (false);
			}
		}

		private void ScrollTheView (bool move)
		{
			// scroll the view up or down
			UIView.BeginAnimations (string.Empty, System.IntPtr.Zero);
			UIView.SetAnimationDuration (0.3);

			CGRect frame = View.Frame;

			if (move) {
				frame.Y = startingY - scroll_amount;
			} else {
				frame.Y = startingY;
				scroll_amount = 0;
			}

			View.Frame = frame;
			UIView.CommitAnimations ();
		}

		#endregion
	}
}

