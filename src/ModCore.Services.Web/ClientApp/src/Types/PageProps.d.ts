import { Application } from "./Application"
import { ModCoreUser } from "./ModCoreUser"

export type PageProps =
{
    user: ModCoreUser | null,
    application: Application
}

export type PagePropsWith<T> = PageProps & T