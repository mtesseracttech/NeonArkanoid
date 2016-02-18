/*
 * This code is derived from Universal Tween Engine (http://code.google.com/p/java-universal-tween-engine/)
 * 
 * @author Aurelien Ribon | http://www.aurelienribon.com/
 */

using TweenEngine;

namespace TweenEngine
{
	public abstract class ITweenAccessor
	{
		/// _GetValues and _SetValues methods are part of a workaround for C# not supporting wildcards in generics.
		/// Because there is no C# equivalent to Java's "TweenAccessor<?>", we can't make a collection of TweenAccessor objects.
		/// To work around this limitation, the library now keeps collections of ITweenAccessor objects. 
		/// The TweenAccessor class implements this interface and adds the templated type.
		/// These methods are proxies to allow the library to call the templated versions.
		internal abstract int _GetValues(object target, int tweenType, float[] returnValues);
		internal abstract void _SetValues(object target, int tweenType, float[] newValues);
	}

	/// <summary>
	/// The TweenAccessor interface lets you interpolate any attribute from any
	/// object.
	/// </summary>
	/// <remarks>
	/// The TweenAccessor interface lets you interpolate any attribute from any
	/// object. Just implement it as you want and register it to the engine by
	/// calling
	/// <see cref="Tween.RegisterAccessor(System.Type{T}, TweenAccessor{T})">Tween.RegisterAccessor(System.Type&lt;T&gt;, TweenAccessor&lt;T&gt;)
	/// 	</see>
	/// .
	/// <p/>
	/// <h2>Example</h2>
	/// The following code snippet presents an example of implementation for tweening
	/// a Particle class. This Particle class is supposed to only define a position
	/// with an "x" and an "y" fields, and their associated getters and setters.
	/// <p/>
	/// <pre>
	/// <code>
	/// public class ParticleAccessor : TweenAccessor<Particle>
	/// {
	///     public static readonly int X = 1;
	///     public static readonly int Y = 2;
	///     public static readonly int XY = 3;
	///     public int GetValues(Particle target, int tweenType, float[] returnValues) 
	///     {
	///         switch (tweenType) 
	///         {
	///             case X: returnValues[0] = target.X; return 1;
	///             case Y: returnValues[0] = target.Y; return 1;
	///             case XY:
	///                 returnValues[0] = target.X;
	///                 returnValues[1] = target.Y;
	///                 return 2;
	///             default: return 0;
	///         }
	///     }
	///     public void setValues(Particle target, int tweenType, float[] newValues) 
	///     {
	///         switch (tweenType) 
	///         {
	///             case X: target.X = newValues[0]; break;
	///             case Y: target.Y = newValues[1]; break;
	///             case XY:
	///                 target.X = newValues[0];
	///                 target.Y = newValues[1];
	///                 break;
	///             default: break;
	///         }
	///     }
	/// }
	/// </code></pre>
	/// Once done, you only need to register this TweenAccessor once to be able to
	/// use it for every Particle objects in your application:
	/// <p/>
	/// <pre>
	/// <code>Tween.registerAccessor(typeof(Particle), new ParticleAccessor());</code>
	/// </pre>
	/// And that's all, the Tween Engine can now work with all your particles!
	/// </remarks>
	/// <author>Aurelien Ribon | http://www.aurelienribon.com/</author>
	public abstract class TweenAccessor<T> : ITweenAccessor 
		where T : class
	{
		/// <summary>
		/// Gets one or many values from the target object associated to the
		/// given tween type.
		/// </summary>
		/// <remarks>
		/// Gets one or many values from the target object associated to the
		/// given tween type. It is used by the Tween Engine to determine starting
		/// values.
		/// </remarks>
		/// <param name="target">The target object of the tween.</param>
		/// <param name="tweenType">An integer representing the tween type.</param>
		/// <param name="returnValues">An array which should be modified by this method.</param>
		/// <returns>The count of modified slots from the returnValues array.</returns>
		public abstract int GetValues(T target, int tweenType, float[] returnValues);

		/// <summary>
		/// This method is called by the Tween Engine each time a running tween
		/// associated with the current target object has been updated.
		/// </summary>
		/// <param name="target">The target object of the tween.</param>
		/// <param name="tweenType">An integer representing the tween type.</param>
		/// <param name="newValues">The new values determined by the Tween Engine.</param>
		public abstract void SetValues(T target, int tweenType, float[] newValues);

		// Implement proxy methods.
		internal sealed override int _GetValues(object target, int tweenType, float[] returnValues)
		{
			return GetValues(target as T, tweenType, returnValues);
		}

		internal sealed override void _SetValues(object target, int tweenType, float[] newValues)
		{
			SetValues(target as T, tweenType, newValues);
		}
	}
}
