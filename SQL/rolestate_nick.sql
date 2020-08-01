-- mcore_rolestate_nicks

create sequence mcore_rolestate_nicks_id_seq;
create table mcore_rolestate_nicks(
    id integer primary key default nextval('mcore_rolestate_nicks_id_seq'),
    member_id bigint not null,
    guild_id bigint not null,
    nickname varchar,
    unique(member_id, guild_id)
);
alter sequence mcore_rolestate_nicks_id_seq owned by mcore_rolestate_nicks.id;