using System;
using System.Collections.Generic;
using System.Text;

using FlacLibSharp.Helpers;

namespace FlacLibSharp {
    /// <summary>
    /// A set of Cue Sheet Tracks
    /// </summary>
    public class CueSheetTrackCollection : IEnumerable<CueSheetTrack> {

        protected List<CueSheetTrack> trackList;

        /// <summary>
        /// Initializes a new empty instance of the CueSheetTrackCollection.
        /// </summary>
        public CueSheetTrackCollection()
        {
            this.trackList = new List<CueSheetTrack>();
        }

        /// <summary>
        /// Adds a track to the collection of tracks.
        /// </summary>
        /// <param name="track"></param>
        /// <remarks>Note that there cannot be more than 100 tracks in the collection.</remarks>
        public void Add(CueSheetTrack track)
        {
            if (this.trackList.Count >= 100)
            {
                throw new FlacLibSharp.Exceptions.FlacLibSharpException("CueSheetTrack Collection can only contain a maximum of 100 tracks.");
            }
            this.trackList.Add(track);
        }

        /// <summary>
        /// Inserts a track at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="track"></param>
        public void Insert(int index, CueSheetTrack track)
        {
            if (this.trackList.Count >= 100)
            {
                throw new FlacLibSharp.Exceptions.FlacLibSharpException("CueSheetTrack Collection can only contain a maximum of 100 tracks.");
            }
            this.trackList.Insert(index, track);
        }

        /// <summary>
        /// Removes the first occurence of a specific Track in the collection.
        /// </summary>
        /// <param name="track"></param>
        public void Remove(CueSheetTrack track)
        {
            this.trackList.Remove(track);
        }

        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            this.trackList.RemoveAt(index);
        }

        /// <summary>
        /// Gets or sets the track at a given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public CueSheetTrack this[int index]
        {
            get
            {
                return this.trackList[index];
            }
        }

        /// <summary>
        /// Gets the number of tracks contains in the collection of tracks.
        /// </summary>
        public int Count
        {
            get { return this.trackList.Count; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection of tracks.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CueSheetTrack> GetEnumerator()
        {
            return trackList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return trackList.GetEnumerator();
        }
    }
}
