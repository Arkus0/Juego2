namespace Arkus.Game.Authoring
{
    /// <summary>
    /// Stable machine vocabulary for HK08B stale whole-world CAS recovery. Recovery is advisory
    /// read context only: clients must explicitly re-plan/dry-run/apply against Current.
    /// </summary>
    public static class WorldConflictRecoveryContract
    {
        public const string SchemaId = "arkus.world-conflict-recovery@1";

        public const string SameLineageReplan = "same-lineage-replan";
        public const string BoundedReinspectionRequired = "bounded-reinspection-required";

        public const string HistoryComplete = "history-complete";
        public const string ExpectedBaseNotInCurrentLineage = "expected-base-not-in-current-lineage";
        public const string RequiredHistoryUnavailable = "required-history-unavailable";
    }
}
