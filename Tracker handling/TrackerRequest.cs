using OversimplifiedTorrent.BencodeParsing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace OversimplifiedTorrent {

    public enum TrackerRequestType { Regular, Start, Complete, Stop }

    public class TrackerRequest {

        private StringBuilder request;

        public void Create(string announceURL, string infoHash, string peerID, string port,
            string uploaded, string downloaded, string left, TrackerRequestType requestEvent, string trackerID) {
            Create(announceURL, infoHash, peerID, port, uploaded, downloaded, left, requestEvent);
            request.Append($"&trackerid={trackerID}");
        }

        public void Create(string announceURL, string infoHash, string peerID, string port,
            string uploaded, string downloaded, string left, TrackerRequestType requestEvent) {
            request = new StringBuilder();
            request.Append($"{announceURL}?info_hash={infoHash}&peer_id={peerID}&port={PeersConnectionsReciver.Port}&uploaded={uploaded}&downloaded={downloaded}&left={left}");
            if (requestEvent != TrackerRequestType.Regular) {
                request.Append($"&event={RequestEventString(requestEvent)}");
            }
        }

        private string RequestEventString(TrackerRequestType requestEvent) {
            switch (requestEvent) {
                case TrackerRequestType.Start:
                    return "started";

                case TrackerRequestType.Stop:
                    return "stopped";

                case TrackerRequestType.Complete:
                    return "completed";

                default:
                    throw new NotImplementedException();
            }
        }

        public TrackerResponse GetResponse() {
            if (request == null) {
                throw new Exception("Request must be initialized");
            }
            HttpWebRequest httpRequest = WebRequest.CreateHttp(request.ToString());
            WebResponse webResponse = httpRequest.GetResponse();
            return TrackerResponseParser.Parse(webResponse.GetResponseStream());
        }
    }
}
