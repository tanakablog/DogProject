using System;
using UnityEngine;

namespace Core.Extension.Private
{                               
    /// <summary>
    /// Transform拡張クラス
    /// </summary>
    public static class TransformExtension
    {

        private static Vector3 vector3;

        #region SetPosition

        public static void SetPosition( this Transform transform, float x, float y, float z )
        {
            vector3.Set( x, y, z );
            transform.position = vector3;
        }

        public static void SetPosition( this Transform transform, float x, float y )
        {
            vector3.Set( x, y, transform.position.z );
            transform.position = vector3;
        }

        public static void SetPositionX( this Transform transform, float x )
        {
            vector3.Set( x, transform.position.y, transform.position.z );
            transform.position = vector3;
        }

        public static void SetPositionY( this Transform transform, float y )
        {
            vector3.Set( transform.position.x, y, transform.position.z );
            transform.position = vector3;
        }

        public static void SetPositionZ( this Transform transform, float z )
        {
            vector3.Set( transform.position.x, transform.position.y, z );
            transform.position = vector3;
        }

        #endregion

        #region AddPosition

        public static void AddPosition( this Transform transform, float x, float y, float z )
        {
            transform.Translate( x, y, z );
        }

        public static void AddPositionX( this Transform transform, float x )
        {
            transform.Translate( x, 0, 0 );
        }

        public static void AddPositionY( this Transform transform, float y )
        {
            transform.Translate( 0, y, 0 );
        }

        public static void AddPositionZ( this Transform transform, float z )
        {
            transform.Translate( 0, 0, z );
        }

        #endregion

        #region SetLocalPosition

        public static void SetLocalPosition( this Transform transform, float x, float y, float z )
        {
            vector3.Set( x, y, z );
            transform.localPosition = vector3;
        }

        public static void SetLocalPosition( this Transform transform, float x, float y )
        {
            vector3.Set( x, y, transform.localPosition.z );
            transform.localPosition = vector3;
        }

        public static void SetLocalPositionX( this Transform transform, float x )
        {
            vector3.Set( x, transform.localPosition.y, transform.localPosition.z );
            transform.localPosition = vector3;
        }

        public static void SetLocalPositionY( this Transform transform, float y )
        {
            vector3.Set( transform.localPosition.x, y, transform.localPosition.z );
            transform.localPosition = vector3;
        }

        public static void SetLocalPositionZ( this Transform transform, float z )
        {
            vector3.Set( transform.localPosition.x, transform.localPosition.y, z );
            transform.localPosition = vector3;
        }

        #endregion

        #region AddLocalPosition

        public static void AddLocalPosition( this Transform transform, float x, float y, float z )
        {
            transform.Translate( x, y, z, Space.Self );
        }

        public static void AddLocalPositionX( this Transform transform, float x )
        {
            transform.Translate( x, 0, 0, Space.Self );
        }

        public static void AddLocalPositionY( this Transform transform, float y )
        {
            transform.Translate( 0, y, 0, Space.Self );
        }

        public static void AddLocalPositionZ( this Transform transform, float z )
        {
            transform.Translate( 0, 0, z, Space.Self );
        }

        public static void AddLocalPosition( this Transform transform, float x, float y, float z, Transform relativeTo )
        {
            transform.Translate( x, y, z, relativeTo );
        }

        public static void AddLocalPositionX( this Transform transform, float x, Transform relativeTo )
        {
            transform.Translate( x, 0, 0, relativeTo );
        }

        public static void AddLocalPositionY( this Transform transform, float y, Transform relativeTo )
        {
            transform.Translate( 0, y, 0, relativeTo );
        }

        public static void AddLocalPositionZ( this Transform transform, float z, Transform relativeTo )
        {
            transform.Translate( 0, 0, z, relativeTo );
        }

        #endregion

        #region SetLocalScale

        public static void SetLocalScale( this Transform transform, float x, float y, float z )
        {
            vector3.Set( x, y, z );
            transform.localScale = vector3;
        }

        public static void SetLocalScaleX( this Transform transform, float x )
        {
            vector3.Set( x, transform.localScale.y, transform.localScale.z );
            transform.localScale = vector3;
        }

        public static void SetLocalScaleY( this Transform transform, float y )
        {
            vector3.Set( transform.localScale.x, y, transform.localScale.z );
            transform.localScale = vector3;
        }

        public static void SetLocalScaleZ( this Transform transform, float z )
        {
            vector3.Set( transform.localScale.x, transform.localScale.y, z );
            transform.localScale = vector3;
        }

        #endregion

        #region AddLocalScale

        public static void AddLocalScale( this Transform transform, float x, float y, float z )
        {
            vector3.Set( transform.localScale.x + x, transform.localScale.y + y, transform.localScale.z + z );
            transform.localScale = vector3;
        }

        public static void AddLocalScaleX( this Transform transform, float x )
        {
            vector3.Set( transform.localScale.x + x, transform.localScale.y, transform.localScale.z );
            transform.localScale = vector3;
        }

        public static void AddLocalScaleY( this Transform transform, float y )
        {
            vector3.Set( transform.localScale.x, transform.localScale.y + y, transform.localScale.z );
            transform.localScale = vector3;
        }

        public static void AddLocalScaleZ( this Transform transform, float z )
        {
            vector3.Set( transform.localScale.x, transform.localScale.y, transform.localScale.z + z );
            transform.localScale = vector3;
        }

        #endregion

        #region SetEulerAngles

        public static void SetEulerAngles( this Transform transform, float x, float y, float z )
        {
            vector3.Set( x, y, z );
            transform.eulerAngles = vector3;
        }

        public static void SetEulerAnglesX( this Transform transform, float x )
        {
            vector3.Set( x, transform.localEulerAngles.y, transform.localEulerAngles.z );
            transform.eulerAngles = vector3;
        }

        public static void SetEulerAnglesY( this Transform transform, float y )
        {
            vector3.Set( transform.localEulerAngles.x, y, transform.localEulerAngles.z );
            transform.eulerAngles = vector3;
        }

        public static void SetEulerAnglesZ( this Transform transform, float z )
        {
            vector3.Set( transform.localEulerAngles.x, transform.localEulerAngles.y, z );
            transform.eulerAngles = vector3;
        }

        #endregion

        #region AddEulerAngles

        public static void AddEulerAngles( this Transform transform, float x, float y, float z )
        {
            vector3.Set( transform.eulerAngles.x + x, transform.eulerAngles.y + y, transform.eulerAngles.z + z );
            transform.eulerAngles = vector3;
        }

        public static void AddEulerAnglesX( this Transform transform, float x )
        {
            vector3.Set( transform.eulerAngles.x + x, transform.eulerAngles.y, transform.eulerAngles.z );
            transform.eulerAngles = vector3;
        }

        public static void AddEulerAnglesY( this Transform transform, float y )
        {
            vector3.Set( transform.eulerAngles.x, transform.eulerAngles.y + y, transform.eulerAngles.z );
            transform.eulerAngles = vector3;
        }

        public static void AddEulerAnglesZ( this Transform transform, float z )
        {
            vector3.Set( transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + z );
            transform.eulerAngles = vector3;
        }

        #endregion

        #region SetLocalEulerAngles

        public static void SetLocalEulerAngles( this Transform transform, float x, float y, float z )
        {
            vector3.Set( x, y, z );
            transform.localEulerAngles = vector3;
        }

        public static void SetLocalEulerAnglesX( this Transform transform, float x )
        {
            vector3.Set( x, transform.localEulerAngles.y, transform.localEulerAngles.z );
            transform.localEulerAngles = vector3;
        }

        public static void SetLocalEulerAnglesY( this Transform transform, float y )
        {
            vector3.Set( transform.localEulerAngles.x, y, transform.localEulerAngles.z );
            transform.localEulerAngles = vector3;
        }

        public static void SetLocalEulerAnglesZ( this Transform transform, float z )
        {
            vector3.Set( transform.localEulerAngles.x, transform.localEulerAngles.y, z );
            transform.localEulerAngles = vector3;
        }

        #endregion

        #region AddLocalEulerAngles

        public static void AddLocalEulerAngles( this Transform transform, float x, float y, float z )
        {
            vector3.Set( transform.localEulerAngles.x + x, transform.localEulerAngles.y + y, transform.localEulerAngles.z + z );
            transform.localEulerAngles = vector3;
        }

        public static void AddLocalEulerAnglesX( this Transform transform, float x )
        {
            vector3.Set( transform.localEulerAngles.x + x, transform.localEulerAngles.y, transform.localEulerAngles.z );
            transform.localEulerAngles = vector3;
        }

        public static void AddLocalEulerAnglesY( this Transform transform, float y )
        {
            vector3.Set( transform.localEulerAngles.x, transform.localEulerAngles.y + y, transform.localEulerAngles.z );
            transform.localEulerAngles = vector3;
        }

        public static void AddLocalEulerAnglesZ( this Transform transform, float z )
        {
            vector3.Set( transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z + z );
            transform.localEulerAngles = vector3;
        }

        #endregion

        #region Lerp

        /// <summary>
        /// ある値まで移行
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="to"></param>
        /// <param name="t"></param>
        public static void Lerp( this Transform transform, Transform to, float t )
        {
            transform.LerpPosition( to.position, t );
            transform.LerpRotate( to.rotation, t );
            transform.LerpScale( to.localScale, t );
        }

        public static void LerpPosition( this Transform transform, Vector3 to, float t )
        {
            transform.position = Vector3.Lerp( transform.position, to, t );
        }

        public static void LerpPosition( this Transform transform, Vector2 to, float t )
        {
            transform.position = Vector3.Lerp( transform.position, new Vector3( to.x, to.y, transform.position.z ), t );
        }

        public static void LerpRotate( this Transform transform, Quaternion to, float t )
        {
            transform.rotation = Quaternion.Lerp( transform.rotation, to, t );
        }

        public static void LerpRotate( this Transform transform, Vector3 to, float t )
        {
            transform.LerpRotate( Quaternion.Euler( to ), t );
        }

        public static void LerpScale( this Transform transform, Vector3 to, float t )
        {
            transform.localScale = Vector3.Lerp( transform.localScale, to, t );
        }

        public static void LerpPositionX( this Transform transform, float to, float t )
        {
            transform.SetPositionX( Mathf.Lerp( transform.position.x, to, t ) );
        }

        public static void LerpPositionY( this Transform transform, float to, float t )
        {
            transform.SetPositionY( Mathf.Lerp( transform.position.y, to, t ) );
        }

        public static void LerpPositionZ( this Transform transform, float to, float t )
        {
            transform.SetPositionZ( Mathf.Lerp( transform.position.z, to, t ) );
        }

        public static void LerpEulerAnglesX( this Transform transform, float to, float t )
        {
            transform.SetEulerAnglesX( Mathf.LerpAngle( transform.eulerAngles.x, to, t ) );
        }

        public static void LerpEulerAnglesY( this Transform transform, float to, float t )
        {
            transform.SetEulerAnglesY( Mathf.LerpAngle( transform.eulerAngles.y, to, t ) );
        }

        public static void LerpEulerAnglesZ( this Transform transform, float to, float t )
        {
            transform.SetEulerAnglesZ( Mathf.LerpAngle( transform.eulerAngles.z, to, t ) );
        }

        public static void LerpLocalScaleX( this Transform transform, float to, float t )
        {
            transform.SetLocalScaleX( Mathf.Lerp( transform.localScale.x, to, t ) );
        }

        public static void LerpScaleY( this Transform transform, float to, float t )
        {
            transform.SetLocalScaleY( Mathf.Lerp( transform.localScale.y, to, t ) );
        }

        public static void LerpScaleZ( this Transform transform, float to, float t )
        {
            transform.SetLocalScaleZ( Mathf.Lerp( transform.localScale.z, to, t ) );
        }
        #endregion

        #region SmoothStep

        /// <summary>
        /// ある値までスムーズに移行
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="to"></param>
        /// <param name="t"></param>
        public static void SmoothStep( this Transform transform, Transform to, float t )
        {
            transform.SmoothStepPosition( to.position, t );
            transform.SmoothStepEulerAngles( to.eulerAngles, t );
            transform.SmoothStepLocalScale( to.localScale, t );
        }

        public static void SmoothStepPosition( this Transform transform, Vector3 to, float t )
        {
            var pos = transform.position;
            var newPositionX = Mathf.SmoothStep( pos.x, to.x, t );
            var newPositionY = Mathf.SmoothStep( pos.y, to.y, t );
            var newPositionZ = Mathf.SmoothStep( pos.z, to.z, t );
            transform.position = new Vector3( newPositionX, newPositionY, newPositionZ );
        }

        public static void SmoothStepPosition( this Transform transform, Vector2 to, float t )
        {
            transform.SmoothStepPosition( new Vector3( to.x, to.y, transform.position.z ), t );
        }

        public static void SmoothStepEulerAngles( this Transform transform, Vector3 to, float t )
        {
            var eulerAngles = transform.eulerAngles;
            var eulerAnglesX = Mathf.SmoothStep( eulerAngles.x, to.x, t );
            var eulerAnglesY = Mathf.SmoothStep( eulerAngles.y, to.y, t );
            var eulerAnglesZ = Mathf.SmoothStep( eulerAngles.z, to.z, t );
            transform.SetEulerAngles( eulerAnglesX, eulerAnglesY, eulerAnglesZ );
        }

        public static void SmoothStepLocalScale( this Transform transform, Vector3 to, float t )
        {
            var localScale = transform.localScale;
            var localScaleX = Mathf.SmoothStep( localScale.x, to.x, t );
            var localScaleY = Mathf.SmoothStep( localScale.y, to.y, t );
            var localScaleZ = Mathf.SmoothStep( localScale.z, to.z, t );
            transform.SetLocalScale( localScaleX, localScaleY, localScaleZ );
        }

        public static void SmoothStepLocalScale( this Transform transform, Vector2 to, float t )
        {
            var localScale = transform.localScale;
            var localScaleX = Mathf.SmoothStep( localScale.x, to.x, t );
            var localScaleY = Mathf.SmoothStep( localScale.y, to.y, t );
            transform.SetLocalScale( localScaleX, localScaleY, transform.localScale.z );
        }

        public static void SmoothStepPositionX( this Transform transform, float to, float t )
        {
            transform.SetPositionX( Mathf.SmoothStep( transform.position.x, to, t ) );
        }

        public static void SmoothStepPositionY( this Transform transform, float to, float t )
        {
            transform.SetPositionY( Mathf.SmoothStep( transform.position.y, to, t ) );
        }

        public static void SmoothStepPositionZ( this Transform transform, float to, float t )
        {
            transform.SetPositionZ( Mathf.SmoothStep( transform.position.z, to, t ) );
        }

        public static void SmoothStepEulerAnglesX( this Transform transform, float to, float t )
        {
            transform.SetEulerAnglesX( Mathf.SmoothStep( transform.eulerAngles.x, to, t ) );
        }

        public static void SmoothStepEulerAnglesY( this Transform transform, float to, float t )
        {
            transform.SetEulerAnglesY( Mathf.SmoothStep( transform.eulerAngles.y, to, t ) );
        }

        public static void SmoothStepEulerAnglesZ( this Transform transform, float to, float t )
        {
            transform.SetEulerAnglesZ( Mathf.SmoothStep( transform.eulerAngles.z, to, t ) );
        }

        public static void SmoothStepLocalScaleX( this Transform transform, float to, float t )
        {
            transform.SetLocalScaleX( Mathf.SmoothStep( transform.localScale.x, to, t ) );
        }

        public static void SmoothStepScaleY( this Transform transform, float to, float t )
        {
            transform.SetLocalScaleY( Mathf.SmoothStep( transform.localScale.y, to, t ) );
        }

        public static void SmoothStepScaleZ( this Transform transform, float to, float t )
        {
            transform.SetLocalScaleZ( Mathf.SmoothStep( transform.localScale.z, to, t ) );
        }
        #endregion

        #region LookAt2D

        /// <summary>
        /// minからmax間の値に変更する
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static void Clamp( this Transform transform, Transform min, Transform max )
        {
            transform.ClampPosition( min.position, max.position );
            transform.ClampEulerAngles( min.eulerAngles, max.eulerAngles );
            transform.ClampLocalScale( min.localScale, max.localScale );
        }

        public static void ClampPosition( this Transform transform, Vector3 min, Vector3 max )
        {
            float x = Mathf.Clamp( transform.position.x, min.x, max.x );
            float y = Mathf.Clamp( transform.position.y, min.y, max.y );
            float z = Mathf.Clamp( transform.position.z, min.z, max.z );

            transform.SetPosition( x, y, z );
        }

        public static void ClampPosition( this Transform transform, Vector2 min, Vector2 max )
        {
            float x = Mathf.Clamp( transform.position.x, min.x, max.x );
            float y = Mathf.Clamp( transform.position.y, min.y, max.y );
            transform.SetPosition( x, y );
        }

        public static void ClampEulerAngles( this Transform transform, Vector3 min, Vector3 max )
        {
            float x = Mathf.Clamp( transform.eulerAngles.x, min.x, max.x );
            float y = Mathf.Clamp( transform.eulerAngles.y, min.y, max.y );
            float z = Mathf.Clamp( transform.eulerAngles.z, min.z, max.z );

            transform.SetEulerAngles( x, y, z );
        }

        public static void ClampLocalScale( this Transform transform, Vector3 min, Vector3 max )
        {
            float x = Mathf.Clamp( transform.localScale.x, min.x, max.x );
            float y = Mathf.Clamp( transform.localScale.y, min.y, max.y );
            float z = Mathf.Clamp( transform.localScale.z, min.z, max.z );

            transform.SetLocalScale( x, y, z );
        }
        public static void ClampPositionX( this Transform transform, float min, float max )
        {
            transform.SetPositionX( Mathf.Clamp( transform.position.x, min, max ) );
        }

        public static void ClampPositionY( this Transform transform, float min, float max )
        {
            transform.SetPositionY( Mathf.Clamp( transform.position.y, min, max ) );
        }

        public static void ClampPositionZ( this Transform transform, float min, float max )
        {
            transform.SetPositionZ( Mathf.Clamp( transform.position.z, min, max ) );
        }
        public static void ClampEulerAnglesX( this Transform transform, float min, float max )
        {
            transform.SetEulerAnglesX( Mathf.Clamp( transform.eulerAngles.x, min, max ) );
        }

        public static void ClampEulerAnglesY( this Transform transform, float min, float max )
        {
            transform.SetEulerAnglesY( Mathf.Clamp( transform.eulerAngles.y, min, max ) );
        }

        public static void ClampEulerAnglesZ( this Transform transform, float min, float max )
        {
            transform.SetEulerAnglesZ( Mathf.Clamp( transform.eulerAngles.z, min, max ) );
        }
        public static void ClampLocalScaleX( this Transform transform, float min, float max )
        {
            transform.SetLocalScaleX( Mathf.Clamp( transform.localScale.x, min, max ) );
        }

        public static void ClampLocalScaleY( this Transform transform, float min, float max )
        {
            transform.SetLocalScaleY( Mathf.Clamp( transform.localScale.y, min, max ) );
        }

        public static void ClampLocalScaleZ( this Transform transform, float min, float max )
        {
            transform.SetLocalScaleZ( Mathf.Clamp( transform.localScale.z, min, max ) );
        }
        #endregion

        #region HashChanged

        /// <summary>
        /// 変更されていたら行う
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="changed"></param>
        public static void HasChanged( this Transform transform, Action changed )
        {
            if( transform.hasChanged )
            {
                changed();
                transform.hasChanged = false;
            }
        }

        private static void HasChanged( this Transform transform, Action<Transform> changed )
        {
            if( transform.hasChanged )
            {
                changed( transform );
                transform.hasChanged = false;
            }
        }

        public static void HasChangedInChildren( this Transform transform, Action<Transform> changed )
        {
            foreach( var child in transform.GetComponentsInChildren<Transform>() )
            {
                child.HasChanged( changed );
            }
        }

        public static void HasChangedInParent( this Transform transform, Action<Transform> changed )
        {
            foreach( var parent in transform.GetComponentsInParent<Transform>() )
            {
                parent.HasChanged( changed );
            }
        }

        #endregion

        #region LookAt2D

        public static void LookAt2D( this Transform transform, Transform target )
        {
            LookAt2D( transform, target.position, Vector3.forward, 0 );
        }

        public static void LookAt2D( this Transform transform, Vector2 target )
        {
            LookAt2D( transform, target, Vector3.forward, 0 );
        }

        public static void LookAt2D( this Transform transform, Transform target, float angle )
        {
            LookAt2D( transform, target.position, Vector3.forward, angle );
        }

        public static void LookAt2D( this Transform transform, Vector2 target, float angle )
        {
            LookAt2D( transform, target, Vector3.forward, angle );
        }

        public static void LookAt2D( this Transform transform, Transform target, Vector3 axis )
        {
            LookAt2D( transform, target.position, axis, 0 );
        }

        public static void LookAt2D( this Transform transform, Vector2 target, Vector3 axis )
        {
            LookAt2D( transform, target, axis, 0 );
        }

        public static void LookAt2D( this Transform transform, Transform target, Vector3 axis, float angle )
        {
            LookAt2D( transform, target.position, axis, angle );
        }

        public static void LookAt2D( this Transform transform, Vector2 target, Vector3 axis, float angle )
        {
            var dir = new Vector2( target.x - transform.position.x, target.y - transform.position.y );
            angle = angle + Mathf.Atan2( dir.y, dir.x ) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis( angle, axis );
        }
        #endregion

        public static void ResetTransform( this Transform transform )
        {
            transform.position = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
    }
}